using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
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
    public class UserCardsRepository : GenericRepository<UserCardsEntity>, IUserCardsRepository
    {
        private readonly DbSet<UserCardsEntity> _dbSet;
        public UserCardsRepository(BankAppContext context): base(context) {
            _dbSet = context.Set<UserCardsEntity>();
        }

        public async Task<List<UserCardsEntity>> GetUserCardsAsync(Guid userId, int pageNumber, int pageSize, Expression<Func<UserCardsEntity, bool>>? searchFilter,
            bool ascending, Expression<Func<UserCardsEntity, object>>? sortValue, List<Expression<Func<UserCardsEntity, bool>>>? filters)
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
            query = query.Where(uc => uc.UserId == userId);
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


        public async Task<List<UserCardsEntity>> GetUnfiltredUserCardsAsync(Guid userId)
        {
            return await _dbSet.Where(uc=>uc.UserId == userId).ToListAsync();
        }

        public async Task<List<UserCardsEntity>> GetAllExpiredUserCardsAsync(Guid userId)
        {
            return await _dbSet.Where(uc => uc.UserId == userId && uc.ExpiryDate<DateOnly.FromDateTime(DateTime.Now))
                .ToListAsync();
        }

        public async Task<int> GetUserCardsCountAsync(Guid userId, Expression<Func<UserCardsEntity, bool>>? searchFilter, 
            List<Expression<Func<UserCardsEntity, bool>>>? filters)
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
            query = query.Where(uc=> uc.Id==userId);
            return await query.CountAsync();
        }

        public async Task<bool> IsCardUnique(Guid userId, Guid cardId, Currency chosenCurrency)
        {
            if(await _dbSet.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CardTariffId == cardId && uc.ChosenCurrency==chosenCurrency) == null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsCardNumberUnique(string cardNumber)
        {
            if(await _dbSet.FirstOrDefaultAsync(uc => uc.CardNumber == cardNumber) == null)
            {
                return true;
            }
            return false;
        }

        public async Task<Guid?> GetCardIdByCardNumberAsync(string cardNumber)
        {
            UserCardsEntity? card = await _dbSet.FirstOrDefaultAsync(uc=>uc.CardNumber==cardNumber);
            if (card == null) return null;
            return card.Id;
        }
    }
}
