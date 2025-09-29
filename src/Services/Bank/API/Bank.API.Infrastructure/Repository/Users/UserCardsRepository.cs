using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.RepositoryContracts.Users;
using Bank.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository.Users
{
    public class UserCardsRepository : GenericRepository<UserCardsEntity>, IUserCardsRepository
    {
        public UserCardsRepository(BankAppContext context): base(context) { }
    }
}
