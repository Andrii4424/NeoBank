using Bank.API.Domain.Abstractions;
using Bank.API.Domain.Enums.CardEnums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities.Cards
{
    public class CardTariffsEntity : IHasId
    {
        [Key]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid BankId { get; private set; }

        public BankEntity Bank { get; private set; }

        [StringLength(40)]
        public string CardName { get; set; }

        public CardType Type { get; set; }

        public CardLevel Level { get; set; }

        public double ValidityPeriod { get; set; }

        public int MaxCreditLimit { get; set; }

        public List<PaymentSystem> EnabledPaymentSystems { get; set; }

        public double? InterestRate { get; set; }

        public List<Currency> EnableCurrency { get; set; }

        public int AnnualMaintenanceCost { get; set; }

        public double P2PInternalCommission { get; set; }

        [StringLength(7)]
        public string CardColor { get; set; }

        [StringLength(6)]
        public string BIN { get; set; }

        public ICollection<UserCardsEntity> UserCards { get; set; }
    }
}
