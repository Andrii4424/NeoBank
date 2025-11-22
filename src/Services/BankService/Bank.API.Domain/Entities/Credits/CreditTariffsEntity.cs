using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities.Credits
{
    public class CreditTariffsEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int MinTermMonths { get; set; }
        public int MaxTermMonths { get; set; }
        public List<Currency> AvaibleCurrencies { get; set; }
    }
}
