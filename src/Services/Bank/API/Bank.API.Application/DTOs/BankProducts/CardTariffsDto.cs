using Bank.API.Domain.Enums.CardEnums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.BankProducts
{
    public class CardTariffsDto :IValidatableObject
    {
        public Guid? Id { get; set; }

        public Guid? BankId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(40)]
        [Display(Name = "Card name")]
        public string CardName { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Card type")]
        public CardType Type { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Card level")]
        public CardLevel Level { get; set; }

        [Range(1, 99, ErrorMessage = "{0} must be between {1} and {2}")]
        [Display(Name = "Validity period")]
        public double ValidityPeriod { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} cant be lesser than 0 or empty")]
        [Display(Name = "Max credit limit")]
        public int MaxCreditLimit { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Payment system")]
        public List<PaymentSystem> EnabledPaymentSystems { get; set; }

        public double? InterestRate { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Curency")]
        public List<Currency> EnableCurrency { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Annual maintenance cost")]
        public int AnnualMaintenanceCost { get; set; }

        [Range(0, 5.00, ErrorMessage = "{0} must be between {1} and {2} according to the rules of the banking system")]
        [Display(Name = "P2P Internal Commission")]
        public double P2PInternalCommission { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(6)]
        [Display(Name = "BIN")]
        public string BIN { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(7)]
        [Display(Name = "Card Color")]
        public string CardColor { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == CardType.Debit && MaxCreditLimit > 0) yield return new ValidationResult("Debit card cant have credit limit");
            if (Type == CardType.Debit && InterestRate >0) yield return new ValidationResult("Debit card cant have interest rate");
            if (ValidityPeriod % 0.5 != 0) yield return new ValidationResult("The card validity period must be a multiple of 1 year " +
                "or half a year (0.5)");
            if (BIN.Length != 6) yield return new ValidationResult("BIN must contain 6 digits");
            if (Type == CardType.Credit && MaxCreditLimit <= 0) yield return new ValidationResult("Credit card must have credit limit");
            if (Type == CardType.Credit && InterestRate <= 0) yield return new ValidationResult("Interest rate cant be 0 or lesser for credit card");
            if(EnableCurrency ==null || EnableCurrency.Count == 0) yield return new ValidationResult("At least one currency has to be chosen");
            if (EnabledPaymentSystems == null || EnabledPaymentSystems.Count == 0) yield return new ValidationResult("At least one payment system has to be chosen");

        }
    }
}
