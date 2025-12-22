using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Credits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts.BankProducts
{
    public interface ICreditTariffsRepository : IGenericRepository<CreditTariffsEntity>
    {
    }
}
