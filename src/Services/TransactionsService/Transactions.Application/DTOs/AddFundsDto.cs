using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Enums;

namespace Transactions.Application.DTOs
{
    public class AddFundsDto
    {
        public Guid CardId {  get; set; }
        public decimal Amount { get; set; }
        public TransactionType OperationType { get; set; }
        public Currency CardCurrency { get; set; }
    }
}
