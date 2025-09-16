using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Identity
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? TaxId { get; set; }

        public string? AvatarPath { get; set; }

        public string? Role { get; set; }

        public bool? IsVerified { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
