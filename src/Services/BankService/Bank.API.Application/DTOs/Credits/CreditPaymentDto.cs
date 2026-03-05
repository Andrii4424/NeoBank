using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Credits
{
    public class CreditPaymentDto
    {
        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Credit Id")]
        public Guid CreditId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cant be lesser than 0")]
        [Display(Name = "Amount")]
        public decimal Amount {  get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }
    }
}
