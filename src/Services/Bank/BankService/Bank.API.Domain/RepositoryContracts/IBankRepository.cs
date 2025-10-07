using Bank.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts
{
    public interface IBankRepository : IGenericRepository<BankEntity>
    {
    }
}
