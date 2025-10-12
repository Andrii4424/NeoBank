using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Entities;

namespace Transactions.Domain.RepositoryContracts
{
    public interface ITransactionRepository : IGenericRepository<TransactionEntity>
    {
        public Task<List<TransactionEntity>> GetTransactions(Guid id, int pageNumber, int pageSize, Expression<Func<TransactionEntity, object>>? sortValue, bool ascending,
            List<Expression<Func<TransactionEntity, bool>>>? filters);
        public Task<int> GetTransactionsCount(Guid id, List<Expression<Func<TransactionEntity, bool>>>? filters);
    }
}
