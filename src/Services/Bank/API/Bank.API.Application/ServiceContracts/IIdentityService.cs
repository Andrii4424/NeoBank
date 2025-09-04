using Bank.API.Application.DTOs.Identity;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts
{
    public interface IIdentityService
    {
        public Task<IdentityResult> RegisterAsync(RegisterDto registerDto);
        public Task<bool> IsEmailUniqueAsync(string email);
        public Task<ApplicationUser> GetUserByEmailAsync(string email);


    }
}
