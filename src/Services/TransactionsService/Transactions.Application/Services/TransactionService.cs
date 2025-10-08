using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.ServiceContracts;
using Transactions.Domain.RepositoryContracts;

namespace Transactions.Application.Services
{
    public class TransactionService : ITransactionService { 

        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }


    }
}
