using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository.BankProducts
{
    public class CreditTariffsRepository: GenericRepository<CreditTariffsEntity>, ICreditTariffsRepository
    {
        public CreditTariffsRepository(BankAppContext context) : base(context) { }
    }
}
