using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Identity
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="{0} has to be provided")]
        [EmailAddress(ErrorMessage = "{0} field has to be in email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
