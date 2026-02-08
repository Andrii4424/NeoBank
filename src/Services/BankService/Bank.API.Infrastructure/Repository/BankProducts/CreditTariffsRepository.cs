using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository.BankProducts
{
    public class CreditTariffsRepository: GenericRepository<CreditTariffsEntity>, ICreditTariffsRepository
    {
        private readonly DbSet<CreditTariffsEntity> _dbSet;

        public CreditTariffsRepository(BankAppContext context) : base(context) {
            _dbSet = context.Set<CreditTariffsEntity>();
        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name) == null;
        }
    }
}
