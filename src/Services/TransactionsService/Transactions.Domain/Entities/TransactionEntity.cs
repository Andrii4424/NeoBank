using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Abstractions;
using Transactions.Domain.Enums;

namespace Transactions.Domain.Entities
{
    public class TransactionEntity :IHasId
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? SenderCardId { get; set; }

        public Guid? SenderId { get; set; }

        public Guid? GetterCardId { get; set; }

        public Guid? GetterId { get; set; }

        public Currency? SenderCurrency { get; set; }

        public Currency? GetterCurrency { get; set; }

        public decimal Amount { get; set; } 

        public decimal Commission {  get; set; }

        public decimal CurrencyExchangeCommission { get; set; }

        public TransactionStatus Status { get; set; }

        public TransactionType Type { get; set; }

        public DateTime? TransactionTime { get; set; }
    }
}
