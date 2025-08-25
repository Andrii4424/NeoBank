using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities
{
    public class BankEntity
    {
        [Key]
        public int Id { get; private set; } = 1;

        public string Name { get; private set; } = "NeoBank";

        public double Rating { get; set; }

        public bool HasLicense { get; set; }

        [StringLength(40)]
        public string BankFounderFullName { get; set; }

        [StringLength(40)]
        public string BankDirectorFullName { get; set; }

        public double Capitalization { get; set; }

        public int EmployeesCount { get; set; }

        public int BlockedClientsCount { get; set; }

        public int ClientsCount { get; set; }

        public int ActiveClientsCount { get; set; }

        [StringLength(100)]
        public string? WebsiteUrl { get; set; }

        [StringLength(20)]
        public string ContactPhone { get; set; }

        public DateTime EstablishedDate { get; }

        [StringLength(100)]
        public string LegalAddress { get; set; }

        public double PercentageCommissionForBuyingCurrency { get; set; }

        public double PercentageCommissionForSellingCurrency { get; set; }

        [StringLength(12)]
        public string SwiftCode { get; set; }

        [StringLength(20)]
        public string MfoCode { get; set; }

        [StringLength(20)]
        public string TaxId { get; set; }

        [StringLength(100)]
        public string ContactEmail { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
