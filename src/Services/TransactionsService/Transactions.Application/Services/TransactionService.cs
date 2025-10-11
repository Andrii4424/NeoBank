using AutoMapper;
using System.Net.Http.Json;
using Transactions.Application.DTOs;
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

        public async Task<OperationResult> MakeP2PTransaction(TransactionDto transaction)
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

                OperationResult balanceCheckResult = await CheckBalance(transaction.SenderCardId.Value, amountWithComission);
            if (!balanceCheckResult.Success) { 
                return balanceCheckResult;
            }

            transaction.Status = TransactionStatus.Pending;

            OperationResult transacrtionResult = await CreateTransaction(transaction);
            if (!transacrtionResult.Success) return transacrtionResult;

            await _rabbitMqMessageBusService.PublishAsync(new CardOperationDto { cardId = transaction.SenderId.Value, amount=amountWithComission},
                _exchange, "balance.update");

            return OperationResult.Ok();
        }

        public async Task<TransactionDto> ExchangeCurrency(TransactionDto transaction)
        {
            transaction = await GetTransactionDtoWithIdentifiers(transaction);


            return transaction;
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

        private async Task<OperationResult> CreateTransaction(TransactionDto transaction)
        {
            TransactionEntity transactionEntity = _mapper.Map<TransactionEntity>(transaction);
            
            await _transactionRepository.AddAsync(transactionEntity);

            await _transactionRepository.SaveAsync();

            return OperationResult.Ok();
        }
    }
}
