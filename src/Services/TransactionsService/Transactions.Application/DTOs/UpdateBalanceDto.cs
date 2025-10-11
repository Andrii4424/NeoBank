using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.DTOs
{
    public class UpdateBalanceDto
    {
        public Guid Id { get; set; }
        public Guid? SenderCardId { get; set; }
        public Guid? GetterCardId { get; set; }
        public double? AmountToReplenish { get; set; }
        public double? AmountToWithdrawn { get; set; }
        public bool? Success { get; set; }
    }
}
