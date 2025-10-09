using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.DTOs
{
    public class TransactionIdentifiers
    {
        public Guid? SenderCardId { get; set; }
        public Guid? SenderId { get; set; }
        public Guid? GetterCardId { get; set; }
        public Guid? GetterId { get; set; }
    }
}
