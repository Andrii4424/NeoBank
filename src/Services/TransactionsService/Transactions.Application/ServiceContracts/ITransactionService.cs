using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.DTOs;

namespace Transactions.Application.ServiceContracts
{
    public interface ITransactionService
    {
        public Task<TransactionDto> ExchangeCurrency(TransactionDto transaction);
        public Task<OperationResult> MakeP2PTransaction(TransactionDto transaction);
        public Task UpdateTransactionStatus(UpdateBalanceDto? transactionDetails);
    }
}
