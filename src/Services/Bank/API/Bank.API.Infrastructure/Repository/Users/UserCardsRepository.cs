using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
using Bank.API.Domain.RepositoryContracts.Users;
using Bank.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<UserCardsEntity>> GetUserCardsAsync(Guid userId)
        {
            return await _dbSet.Where(uc => uc.UserId == userId).ToListAsync();
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
    }
}
