using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.BankProducts
{
    public class CreateUserCardDto :IValidatableObject
    {
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Card Tariffs")]
        public Guid CardTariffId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Currency")]
        public Currency ChosenCurrency { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Payment System")]
        public PaymentSystem ChosenPaymentSystem { get; set; }


        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Pin")]
        public string Pin { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Pin.Length!=4) yield return new ValidationResult("Pin must be 4 characters long");
        }
    }
}
