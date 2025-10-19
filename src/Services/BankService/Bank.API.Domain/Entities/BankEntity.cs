using Bank.API.Domain.Abstractions;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities
{
    public class BankEntity : IHasId
    {
        [Key]
        public Guid Id { get; private set; } = Guid.Parse("E2A4A522-8486-46F7-9437-5F5B7E539502");

        public string Name { get; private set; } = "NeoBank";

        public double Rating { get; set; }

        public bool HasLicense { get; set; }

        [StringLength(40)]
        public string BankFounderFullName { get; set; }

        [StringLength(40)]
        public string BankDirectorFullName { get; set; }

        public long Capitalization { get; set; }

        public int EmployeesCount { get; set; }

        public int BlockedClientsCount { get; set; }

        public int ClientsCount { get; set; }

        public int ActiveClientsCount { get; set; }

        [StringLength(20)]
        public string ContactPhone { get; set; }

        public DateOnly EstablishedDate { get; set; }

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

        public DateOnly UpdatedAt { get; set; }

        public ICollection<CardTariffsEntity> Cards { get; set; }
        public ICollection<VacancyEntity> Vacancies { get; set; }

    }
}
