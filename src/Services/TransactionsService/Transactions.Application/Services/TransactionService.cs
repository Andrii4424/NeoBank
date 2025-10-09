using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transactions.Application.DTOs;
using Transactions.Application.ServiceContracts;
using Transactions.Domain.RepositoryContracts;

namespace Transactions.Application.Services
{
    public class TransactionService : ITransactionService { 

        private readonly ITransactionRepository _transactionRepository;
        private readonly HttpClient _client;

        public TransactionService(ITransactionRepository transactionRepository, IHttpClientFactory httpClientFactory)
        {
            _transactionRepository = transactionRepository;
            _client = httpClientFactory.CreateClient("BankApi");
        }

        public async Task<TransactionDto> ExchangeCurrency(TransactionDto transaction)
        {
            transaction = await GetTransactionDtoWithIdentifiers(transaction);


            return transaction;
        }

        private async Task<TransactionDto> GetTransactionDtoWithIdentifiers(TransactionDto transaction)
        {
            Console.WriteLine($"Full URL: {new Uri(_client.BaseAddress!, "Transaction/GetTransactionDetails")}");

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

    }
}
