using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Entities;
using Transactions.Domain.RepositoryContracts;
using Transactions.Infrastructure.Data;

namespace Transactions.Infrastructure.Repository
{
    public class TransactionRepository : GenericRepository<TransactionEntity>, ITransactionRepository
    {
        private readonly DbSet<TransactionEntity> _dbSet;

        public TransactionRepository(TransactionContext context) : base(context)
        {
            _dbSet = context.Set<TransactionEntity>();
        }

        public async Task<List<TransactionEntity>> GetTransactions(Guid id, int pageNumber, int pageSize, Expression<Func<TransactionEntity, object>>? sortValue, bool ascending, 
            List<Expression<Func<TransactionEntity, bool>>>? filters)
        {
            var query = _dbSet.AsQueryable();

            query = query.Where(t => t.SenderCardId == id || t.GetterCardId == id);

            if(sortValue != null)
            {
                query = ascending ? query.OrderBy(sortValue).ThenBy(t => t.Id) : query.OrderByDescending(sortValue).ThenBy(t => t.Id);

            }
            else
            {
                query = query.OrderBy(t => t.Id);
            }

            if (filters != null)
            {
                filters = filters.Where(val => val != null).ToList();
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<int> GetTransactionsCount(Guid id, List<Expression<Func<TransactionEntity, bool>>>? filters)
        {
            var query = _dbSet.AsQueryable();

            query = query.Where(t => t.SenderCardId == id || t.GetterCardId == id);

            if (filters != null)
            {
                filters = filters.Where(val => val != null).ToList();
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            return await query.CountAsync();
        }
    }
}
