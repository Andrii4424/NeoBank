using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Identity
{
    public class ChangePasswordDto
    {
        public string? Email { get; set; }
        public string? RefreshCode { get; set; }
        public string? NewPassword { get; set; }

    }
}
