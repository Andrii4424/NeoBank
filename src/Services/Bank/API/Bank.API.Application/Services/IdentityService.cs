using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace Bank.API.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
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

        public async Task<AuthenticationResponse> GetJwt(ApplicationUser user)
        {
            Claim[] claim = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(user.Id)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expiration_minutes"])),
                signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(jwtSecurityToken);

            return new AuthenticationResponse
            {
                AccessToken = token,
                ExpiresOn = jwtSecurityToken.ValidTo
            };
        }
    }
}
