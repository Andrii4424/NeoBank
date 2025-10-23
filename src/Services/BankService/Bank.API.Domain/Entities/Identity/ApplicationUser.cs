using Bank.API.Domain.Entities.Cards;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? TaxId { get; set; }

        public string? AvatarPath { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public bool? IsVerified { get; set; }

        public string? JobTitle { get; set; }

        public string? JobCategory { get; set; }

        public double? Salary { get; set; }

        public ICollection<UserCardsEntity> UserCards { get; set; }

        [StringLength(6)]
        public string? RefreshCode { get; set; }

        public DateTime? RefreshCodeExpiryTime { get; set; }

    }
}
