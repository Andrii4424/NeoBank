using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Enums;

namespace Transactions.Domain.Entities
{
    public class TransactionEntity
    {
        public Guid Id { get; set; }

        public Guid? SenderCardId { get; set; }

        public Guid? GetterCardId { get; set; }

        public decimal Amount { get; set; } 

        public decimal Commission {  get; set; }

        public TransactionStatus Status { get; set; }

        public TransactionType Type { get; set; }
    }
}
