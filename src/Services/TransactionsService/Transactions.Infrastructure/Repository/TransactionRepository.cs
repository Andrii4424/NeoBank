using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Entities;
using Transactions.Domain.RepositoryContracts;
using Transactions.Infrastructure.Data;

namespace Transactions.Infrastructure.Repository
{
    public class TransactionRepository : GenericRepository<TransactionEntity>
    {
        public TransactionRepository(TransactionContext context) : base(context)
        {

        }
    }
}
