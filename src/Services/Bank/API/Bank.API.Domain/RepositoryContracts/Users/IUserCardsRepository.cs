using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts.Users
{
    public interface IUserCardsRepository : IGenericRepository<UserCardsEntity>
    {
        public Task<List<UserCardsEntity>> GetUserCardsAsync(Guid userId, int pageNumber, int pageSize, Expression<Func<UserCardsEntity, bool>>? searchFilter,
            bool ascending, Expression<Func<UserCardsEntity, object>>? sortValue, List<Expression<Func<UserCardsEntity, bool>>>? filters);
        public Task<bool> IsCardUnique(Guid userId, Guid cardId, Currency chosenCurrency);
        public Task<bool> IsCardNumberUnique(string cardNumber);
        public Task<int> GetUserCardsCountAsync(Guid userId, Expression<Func<UserCardsEntity, bool>>? searchFilter,
            List<Expression<Func<UserCardsEntity, bool>>>? filters);
    }
}
