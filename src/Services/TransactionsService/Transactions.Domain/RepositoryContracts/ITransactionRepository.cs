using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Entities;

namespace Transactions.Domain.RepositoryContracts
{
    public interface ITransactionRepository : IGenericRepository<TransactionEntity>
    {
    }
}
