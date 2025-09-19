using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTO
{
    public class BankDto
    {
        public Guid Id { get; private set; } = Guid.Parse("E2A4A522-8486-46F7-9437-5F5B7E539502");

        public string Name { get; private set; } = "NeoBank";

        [Required(ErrorMessage ="{0} has to be provided")]
        [Range(1.0, 5.0, ErrorMessage = "{0} must be between {1} and {2}")]
        [Display(Name= "Rating")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Display(Name = "Has license")]
        public bool HasLicense { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(40, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Bank Founder Full Name")]
        public string BankFounderFullName { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(40, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Bank Director Full Name")]
        public string BankDirectorFullName { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cant be {1} or lesser")]
        [Display(Name = "Capitalization")]
        public long Capitalization { get; set; }

        [BindNever]
        public int EmployeesCount { get; set; }

        [BindNever]
        public int BlockedClientsCount { get; set; }

        [BindNever]
        public int ClientsCount { get; set; }

        [BindNever]
        public int ActiveClientsCount { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(20, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [BindNever]
        public DateOnly EstablishedDate { get; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(100, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Legal Address")]
        public string LegalAddress { get; set; }

        [Range(0, 10.0, ErrorMessage = "{0} must be between {1} and {2}")]
        [Display(Name = "Percentage Commission For Buying Currency")]
        public double PercentageCommissionForBuyingCurrency { get; set; }

        [Range(0, 10.0, ErrorMessage = "{0} must be between {1} and {2}")]
        [Display(Name = "Percentage Commission For Selling Currency ")]
        public double PercentageCommissionForSellingCurrency { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(12, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Swift Code")]
        public string SwiftCode { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(20, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Mfo Code")]
        public string MfoCode { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(20, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Tax Id")]
        public string TaxId { get; set; }

        [Required(ErrorMessage = "{0} has to be provided")]
        [StringLength(100, ErrorMessage = "{0} character limit exceeded {1}")]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [BindNever]
        public DateOnly UpdatedAt { get; set; }
    }
}
