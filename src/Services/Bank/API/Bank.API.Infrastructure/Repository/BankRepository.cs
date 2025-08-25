using Bank.API.Domain.Entities;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository
{
    public class BankRepository : GenericRepository<BankEntity>, IBankRepository
    {
        public BankRepository(BankAppContext context) : base(context) { }

    }
}
