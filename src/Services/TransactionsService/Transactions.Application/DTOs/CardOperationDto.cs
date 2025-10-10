using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.DTOs
{
    public class CardOperationDto
    {
        public Guid cardId { get; set; }
        public decimal amount { get; set; }
    }
}
