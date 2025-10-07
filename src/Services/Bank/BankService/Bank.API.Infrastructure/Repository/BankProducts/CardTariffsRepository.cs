using Bank.API.Domain.Entities.Cards;
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
    public class CardTariffsRepository : GenericRepository<CardTariffsEntity>, ICardTariffsRepository
    {
        private readonly DbSet<CardTariffsEntity> _dbSet;

        public CardTariffsRepository (BankAppContext context): base(context) {
            _dbSet = context.Set<CardTariffsEntity>();

        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CardName == name)==null;
        }
    }
}
