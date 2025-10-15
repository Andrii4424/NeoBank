using AutoMapper;
using System.Net.Http.Json;
using Transactions.Application.DTOs;
using Transactions.Application.Filters;
using Transactions.Application.Helpers;
using Transactions.Application.ServiceContracts;
using Transactions.Domain.Entities;
using Transactions.Domain.Enums;
using Transactions.Domain.RepositoryContracts;

namespace Transactions.Application.Services
{
    public class TransactionService : ITransactionService { 

        private readonly ITransactionRepository _transactionRepository;
        private readonly HttpClient _client;
        private readonly IRabbitMqProducerService _rabbitMqMessageBusService;
        private readonly IMapper _mapper;
        private readonly string _exchange = "bank.transaction";

        public TransactionService(ITransactionRepository transactionRepository, IHttpClientFactory httpClientFactory, 
            IRabbitMqProducerService rabbitMqMessageBusService, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _client = httpClientFactory.CreateClient("BankApi");
            _rabbitMqMessageBusService = rabbitMqMessageBusService;
            _mapper = mapper;
        }

        //Read Methods
        public async Task<PageResult<TransactionDto>> GetTransactions(Guid cardId, TransactionFilter? filters)
        {
            if (filters == null)
            {
                filters = new TransactionFilter();
            }
            if(filters.PageNumber ==null) filters.PageNumber = 1;

            Filters<TransactionEntity> generalFilters = filters.ToGeneralFilters(cardId);

            List<TransactionEntity> transactions= await _transactionRepository.GetTransactions(cardId, generalFilters.PageNumber.Value, generalFilters.PageSize, 
                generalFilters.SortExpression, generalFilters.Ascending.Value, generalFilters.FiltersExpression);


            return new PageResult<TransactionDto>(_mapper.Map<List<TransactionDto>>(transactions), await _transactionRepository.GetTransactionsCount(cardId, 
                generalFilters.FiltersExpression), filters.PageNumber.Value, filters.PageSize);
        } 

        public async Task<double> GetComissionRate(Guid cardId)
        {
            return await GetOperationComission(cardId, TransactionType.P2P);
        }

        public async Task<Guid?> GetCardIdByCardNumberAsync(string cardNumber)
        {
            var response = await _client.GetAsync($"Transaction/GetCardGuidByCardNumber/{cardNumber}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            else
            {
                Guid parsedGuid;
                if(Guid.TryParse(await response.Content.ReadAsStringAsync(), out parsedGuid) == false) return null;
                return parsedGuid;
            }
        }

        //Create Methods
        public async Task<OperationResult> MakeTransaction(TransactionDto transaction)
        {
            transaction = await GetTransactionDtoWithIdentifiers(transaction);
            if (transaction.SenderCardId == null)
            {
                return OperationResult.Error("Wrong sender card!");
            }
            else if(transaction.GetterCardId == null)
            {
                return OperationResult.Error("Wrong getter card!");
            }
            else if (transaction.Amount<=0)
            {
                return OperationResult.Error("Amount of transfer must be more than 0");
            }
            double comissionPercentage = await GetOperationComission(transaction.SenderCardId.Value, transaction.Type);
            decimal amountWithComission = transaction.Amount;
            if (comissionPercentage > 0) {
                amountWithComission = amountWithComission * (((decimal)comissionPercentage / 100)+1);
                transaction.Commission= amountWithComission - transaction.Amount;
            }
            else
            {
                transaction.Commission = 0;
            }
            transaction.TransactionTime = DateTime.UtcNow;

            OperationResult balanceCheckResult = await CheckBalance(transaction.SenderCardId.Value, amountWithComission);
            if (!balanceCheckResult.Success) { 
                if(balanceCheckResult.ErrorMessage== "Influent balance")
                {
                    transaction.Status = TransactionStatus.Failed;
                    await CreateTransaction(transaction);
                }

                return balanceCheckResult;
            }

            if (transaction.GetterCurrency != transaction.SenderCurrency)
            {
                transaction.AmountToReceive = (decimal)await GetAmountAfterExchange(new ExchangeCurrencyDto { From=transaction.SenderCurrency, To=transaction.GetterCurrency, 
                Amount = (double)transaction.Amount});
            }
            else
            {
                transaction.AmountToReceive = transaction.Amount;
            }
            transaction.Status = TransactionStatus.Pending;

            Guid transactionId = await CreateTransaction(transaction);

            await _rabbitMqMessageBusService.PublishAsync(new UpdateBalanceDto { Id= transactionId, SenderCardId=transaction.SenderCardId,
            GetterCardId =transaction.GetterCardId, AmountToReplenish = (double)transaction.AmountToReceive, AmountToWithdrawn = ((double)amountWithComission)*(-1),
            Success=null}, _exchange, "balance.update");

            return OperationResult.Ok();
        }

        public async Task UpdateTransactionStatus(UpdateBalanceDto? transactionDetails)
        {
            if (transactionDetails == null) throw new ArgumentNullException("Resoponse from update balance service is invalid. Transaction Id didnt provided");
            TransactionEntity? transaction = await _transactionRepository.GetValueByIdAsync(transactionDetails.Id);
            if (transaction == null) throw new ArgumentException("Resoponse from update balance service is invalid. Wrong transaction id, transaction with id doesnt exist");

            if (transactionDetails.Success == true) { 
                transaction.Status = TransactionStatus.Completed;
            }
            else
            {
                transaction.Status = TransactionStatus.Failed;
            }

            _transactionRepository.UpdateObject(transaction);
            await _transactionRepository.SaveAsync();
        }

        private async Task<TransactionDto> GetTransactionDtoWithIdentifiers(TransactionDto transaction)
        {
            var response = await _client.PostAsJsonAsync("Transaction/GetTransactionDetails", new TransactionIdentifiers
            {
                GetterCardId = transaction.GetterCardId,
                SenderCardId = transaction.SenderCardId
            });
            TransactionIdentifiers transactionIdentifiers = await response.Content.ReadFromJsonAsync<TransactionIdentifiers>();

            transaction.SenderId = transactionIdentifiers.SenderId;
            transaction.GetterId = transactionIdentifiers.GetterId;
            transaction.SenderCurrency = transactionIdentifiers.SenderCurrency;
            transaction.GetterCurrency = transactionIdentifiers.GetterCurrency;

            return transaction;
        }

        private async Task<OperationResult> CheckBalance(Guid cardId, decimal amount)
        {
            var response = await _client.GetAsync($"Transaction/CheckBalance/{cardId}/check?amount={amount}");
            if(!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return OperationResult.Error(error);
            }

            bool result = await response.Content.ReadFromJsonAsync<bool>();

            if (result) return OperationResult.Ok();
            else return OperationResult.Error("Influent balance");
        }

        private async Task<double> GetOperationComission(Guid cardId, TransactionType type)
        {
            if(type==TransactionType.P2P)
            {
                var response = await _client.GetAsync($"Transaction/CheckBalance/{cardId}");
                return await response.Content.ReadFromJsonAsync<double>();
            }
            return 0;
        }

        private async Task<double> GetAmountAfterExchange(ExchangeCurrencyDto exchangeParams)
        {
            if(exchangeParams.From == null || exchangeParams.To ==null)
            {
                throw new ArgumentNullException("Currency is not provided for exchange");
            }

            var response = await _client.GetAsync($"Currency/GetExchangedValue?from={exchangeParams.From}&to={exchangeParams.To}&amount={exchangeParams.Amount}");
            if (!response.IsSuccessStatusCode) {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return await response.Content.ReadFromJsonAsync<double>();
        }

        private async Task<Guid> CreateTransaction(TransactionDto transaction)
        {
            TransactionEntity transactionEntity = _mapper.Map<TransactionEntity>(transaction);
            
            await _transactionRepository.AddAsync(transactionEntity);

            await _transactionRepository.SaveAsync();

            return transactionEntity.Id;
        }
    }
}
