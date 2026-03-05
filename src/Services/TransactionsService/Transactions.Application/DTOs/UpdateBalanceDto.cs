using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Enums;

namespace Transactions.Application.DTOs
{
    public class UpdateBalanceDto
    {
        public Guid Id { get; set; }
        public Guid? SenderCardId { get; set; }
        public Guid? GetterCardId { get; set; }
        public Guid? GetterId { get; set; }
        public Guid? CreditTariffsId { get; set; }
        public Currency? TransactionGetterCurrency { get; set; }
        public Guid? UserCreditId { get; set; }
        public int? TermMonths { get; set; }
        public double? AmountToReplenish { get; set; }
        public double? AmountToWithdrawn { get; set; }
        public bool? Success { get; set; }
        public bool? IsCreditPayment { get; set; }
    }
}
