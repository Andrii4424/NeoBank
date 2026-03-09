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
    public class OpenUserCreditDto
    {

        public Guid UserId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Credit Tariff")]
        public Guid CreditTariffId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(1, double.MaxValue, ErrorMessage = "{0} must be greater than 0")]
        [Display(Name = "Credit Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be greater than 0")]
        [Display(Name = "Term Months")]
        public int TermMonths { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Currency")]
        public Currency Currency { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Enrollment Card Number")]
        public string EnrollmentCardNumber { get; set; }

    }
}
