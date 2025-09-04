using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            if(!await IsEmailUniqueAsync(registerDto.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email is already in use." });
            }
            ApplicationUser user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

            return result;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            if(IsEmailUniqueAsync(email).Result)
            {
                throw new ArgumentException("User not found");
            }
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
