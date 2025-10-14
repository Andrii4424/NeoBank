using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.DTOs;
using Transactions.Application.Filters;
using Transactions.Application.Helpers;

namespace Transactions.Application.ServiceContracts
{
    public interface ITransactionService
    {
        public Task<OperationResult> MakeTransaction(TransactionDto transaction);
        public Task UpdateTransactionStatus(UpdateBalanceDto? transactionDetails);
        public Task<double> GetComissionRate(Guid cardId);
        public Task<PageResult<TransactionDto>> GetTransactions(Guid cardId, TransactionFilter? filters);
    }
}
