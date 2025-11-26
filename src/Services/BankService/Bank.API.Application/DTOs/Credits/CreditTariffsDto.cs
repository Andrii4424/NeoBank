using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Credits
{
    public class CreditTariffsDto :IValidatableObject
    {
        public Guid? Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Tariffs name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cant be lesser than 0")]
        [Display(Name = "Interest Rate")]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(0, int.MaxValue, ErrorMessage ="{0} cant be lesser than 0")]
        [Display(Name = "Min Amount")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cant be lesser than 0")]
        [Display(Name = "Max Amount")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cant be lesser than 0")]
        [Display(Name = "Min Term Months")]
        public int MinTermMonths { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cant be lesser than 0")]
        [Display(Name = "Max Term Months")]
        public int MaxTermMonths { get; set; }

        public List<Currency>? AvaibleCurrencies { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinAmount > MaxAmount) yield return new ValidationResult("Minimum credit amount cannot be greater than than maximum credit amount");
            if (MinTermMonths > MaxTermMonths) yield return new ValidationResult("Minimum credit term cannot be longer than than maximum credit term");
        }
    }
}
