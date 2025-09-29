using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts.Users
{
    public interface IUserCardsRepository : IGenericRepository<UserCardsEntity>
    {
        public Task<List<UserCardsEntity>> GetUserCardsAsync(Guid userId);
        public Task<bool> IsCardUnique(Guid userId, Guid cardId, Currency chosenCurrency);
        public Task<bool> IsCardNumberUnique(string cardNumber);
    }
}
