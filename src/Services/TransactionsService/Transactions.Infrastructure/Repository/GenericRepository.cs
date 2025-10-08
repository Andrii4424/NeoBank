using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Abstractions;
using Transactions.Domain.RepositoryContracts;
using Transactions.Infrastructure.Data;

namespace Transactions.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IHasId
    {
        private readonly TransactionContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(TransactionContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<Boolean> IsObjectIdExists(Guid id)
        {
            return await _dbSet.AnyAsync(obj => obj.Id == id);
        }

        public async Task<IEnumerable<T>?> GetAllValuesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T?> GetValueByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void UpdateObject(T entity)
        {
            _dbSet.Update(entity);
        }

        public void DeleteElement(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> GetFilteredListAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? searchFilter,
            bool ascending, Expression<Func<T, object>>? sortValue, List<Expression<Func<T, bool>>>? filters)
        {
            var query = _dbSet.AsQueryable();
            if (searchFilter != null) query = query.Where(searchFilter);
            if (filters != null)
            {
                filters = filters.Where(v => v != null).ToList();
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            if (sortValue != null)
            {
                query = ascending ? query.OrderBy(sortValue).ThenBy(obj => obj.Id) : query.OrderByDescending(sortValue).ThenBy(obj => obj.Id);
            }
            else
            {
                query = query.OrderBy(obj => obj.Id);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>>? searchFilter, List<Expression<Func<T, bool>>>? filters)
        {
            var query = _dbSet.AsQueryable();
            if (searchFilter != null) query = query.Where(searchFilter);
            if (filters != null)
            {
                filters = filters.Where(v => v != null).ToList();
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            return await query.CountAsync();
        }
    }
}
