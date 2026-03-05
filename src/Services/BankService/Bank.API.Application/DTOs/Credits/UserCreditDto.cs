using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.Enums.CardEnums;
using Bank.API.Domain.Enums.CreditEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Credits
{
    public class UserCreditDto
    {
        public Guid? Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Credit Tariff Id")]
        public Guid CreditTariffId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(1, double.MaxValue, ErrorMessage = "{0} must be greater than 0")]
        [Display(Name = "Credit Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be greater than 0")]
        [Display(Name = "Term Months")]
        public int TermMonths { get; set; }

        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        public Currency Currency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CreditStatus Status { get; set; } = CreditStatus.Active;

        public decimal MonthlyPayment { get; set; }

        public decimal RemainingDebt { get; set; }

        public decimal CurrentMonthAmountDue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public CreditTariffsDto? CreditTariffs { get; private set; }

    }
}
