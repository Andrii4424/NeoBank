using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users.CardOperations
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid? SenderCardId { get; set; }
        public Guid? GetterCardId { get; set; }
        public double? AmountToReplenish { get; set; }
        public double? AmountToWithdrawn { get; set; }
        public bool? Success { get; set; }
    }
}
