using Bank.API.Domain.Entities.Credits;
using System.Linq.Expressions;


namespace Bank.API.Domain.RepositoryContracts.Users
{
    public interface IUserCreditsRepository : IGenericRepository<UserCreditEntity>
    {
        public Task<List<UserCreditEntity>> GetUserCredits(Guid userId);
        public Task<List<UserCreditEntity>> GetFilteredCardsAsync(int pageNumber, int pageSize, Expression<Func<UserCreditEntity, bool>>? searchFilter,
            bool ascending, Expression<Func<UserCreditEntity, object>>? sortValue, List<Expression<Func<UserCreditEntity, bool>>>? filters, bool icnludeTariffs);
        public Task<List<UserCreditEntity>> GetActiveCreditsAsync();
    }
}