using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transactions.Application.DTOs;
using Transactions.Application.ServiceContracts;
using Transactions.Domain.Enums;
using Transactions.Domain.RepositoryContracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Transactions.Application.Services
{
    public class TransactionService : ITransactionService { 

        private readonly ITransactionRepository _transactionRepository;
        private readonly HttpClient _client;
        private readonly IRabbitMqProducerService _rabbitMqMessageBusService;
        private readonly string _exchange = "bank.transaction";

        public TransactionService(ITransactionRepository transactionRepository, IHttpClientFactory httpClientFactory, 
            IRabbitMqProducerService rabbitMqMessageBusService)
        {
            _transactionRepository = transactionRepository;
            _client = httpClientFactory.CreateClient("BankApi");
            _rabbitMqMessageBusService = rabbitMqMessageBusService;
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
            double comission = await GetOperationComission(transaction.SenderCardId.Value, transaction.Type);
            decimal amountWithComission = transaction.Amount;
            if (comission > 0) {
                amountWithComission = amountWithComission * (((decimal)comission/100)+1);
            }

            OperationResult balanceCheckResult = await CheckBalance(transaction.SenderCardId.Value, amountWithComission);
            if (!balanceCheckResult.Success) { 
                return balanceCheckResult;
            }
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
    }
}
