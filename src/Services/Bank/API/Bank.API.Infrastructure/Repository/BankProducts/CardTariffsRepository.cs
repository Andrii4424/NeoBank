using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository.BankProducts
{
    public class CardTariffsRepository : GenericRepository<CardTariffsEntity>, ICardTariffsRepository
    {
        public CardTariffsRepository (BankAppContext context): base(context) {}
    }
}
