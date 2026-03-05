using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.Entities.Users;
using Bank.API.Domain.RepositoryContracts.Users;
using Bank.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository.Users
{
    public class UserCreditsRepository : GenericRepository<UserCreditEntity>, IUserCreditsRepository
    {
        private readonly DbSet<UserCreditEntity> _dbSet;
        public UserCreditsRepository(BankAppContext context) : base(context)
        {
            _dbSet = context.Set<UserCreditEntity>();
        }

        public async Task<List<UserCreditEntity>> GetUserCredits(Guid userId)
        {
            return await _dbSet.Where(uc => uc.UserId == userId).ToListAsync();
        }

        public async Task<List<UserCreditEntity>> GetFilteredCardsAsync(int pageNumber, int pageSize, Expression<Func<UserCreditEntity, bool>>? searchFilter,
            bool ascending, Expression<Func<UserCreditEntity, object>>? sortValue, List<Expression<Func<UserCreditEntity, bool>>>? filters, bool icnludeTariffs)
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
            if (icnludeTariffs)
            {
                query = query.Include(uc => uc.CreditTariffs);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<UserCreditEntity>> GetActiveCreditsAsync(){
            return await _dbSet.Where(uc => uc.Status == Domain.Enums.CreditEnums.CreditStatus.Active).ToListAsync();
        }
    }
}
