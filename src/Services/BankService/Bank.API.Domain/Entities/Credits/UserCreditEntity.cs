using Bank.API.Domain.Abstractions;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.Enums.CardEnums;
using Bank.API.Domain.Enums.CreditEnums;

namespace Bank.API.Domain.Entities.Credits
{
    public class UserCreditEntity : IHasId
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid CreditTariffId { get; set; }

        public decimal Amount { get; set; }
        public int TermMonths { get; set; }

        public decimal InterestRate { get; set; }

        public Currency Currency { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public CreditStatus Status { get; set; }

        public decimal MonthlyPayment { get; set; }
        public decimal RemainingDebt { get; set; }

        public decimal? CurrentMonthAmountDue { get; set; }
        public DateOnly? CurrentPaymentDate { get; set; }
        public decimal? AmountToClose { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsClosed { get; set; }
        public DateTime? CloseTime { get; set; }

        public ApplicationUser User { get; private set; }
        public CreditTariffsEntity CreditTariffs { get; private set; }

    }

}
