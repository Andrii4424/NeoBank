using Bank.API.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts
{
    public interface IGenericRepository<T> where T : class, IHasId
    {
        public Task<Boolean> IsObjectIdExists(Guid id);

        public Task<IEnumerable<T>?> GetAllValuesAsync();

        public Task<T?> GetValueByIdAsync(Guid id);

        public Task AddAsync(T entity);

        public void UpdateObject(T entity);

        public void DeleteElement(T entity);

        public Task<List<T>> GetFilteredListAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? searchFilter,
            bool ascending, Expression<Func<T, object>>? sortValue, List<Expression<Func<T, bool>>>? filters);

        public Task<int> GetCountAsync(Expression<Func<T, bool>>? searchFilter, List<Expression<Func<T, bool>>>? filters);

        public Task SaveAsync();
    }
}
