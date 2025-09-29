using Bank.API.Domain.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts.Users
{
    public interface IUserCardsRepository : IGenericRepository<UserCardsEntity>
    {
    }
}
