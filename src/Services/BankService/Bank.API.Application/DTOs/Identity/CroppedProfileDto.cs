using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Identity
{
    public class CroppedProfileDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? AvatarPath { get; set; }

        public string? Role { get; set; }

        public bool? IsVerified { get; set; }
    }
}
