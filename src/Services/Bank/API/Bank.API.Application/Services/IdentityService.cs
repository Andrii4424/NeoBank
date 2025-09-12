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
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Bank.API.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        //Auth
        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            if (!await IsEmailUniqueAsync(registerDto.Email))
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

        public async Task LogoutAsync(ApplicationUser? user)
        {
            if (user != null) { 
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            if (IsEmailUniqueAsync(email).Result)
            {
                throw new ArgumentException("User not found");
            }
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<AuthenticationResponse> GetAccessToken(ApplicationUser user)
        {
            //Access Token
            Claim[] claim = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(user.Id)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(),ClaimValueTypes.Integer64)
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["AccessToken:Key"]!));

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["AccessToken:Issuer"],
                audience: _configuration["AccessToken:Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["AccessToken:Expiration_minutes"])),
                signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(jwtSecurityToken);

            //Refresh Token
            string refreshToken = GenerateRefreshToken();
            await RecordRefreshToken(refreshToken, user);

            return new AuthenticationResponse
            {
                AccessToken = token,
                AccessExpiresOn = jwtSecurityToken.ValidTo,
                RefreshToken = refreshToken,
                RefreshExpiresOn = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["RefreshToken:Expiration_days"]))
            };
        }

        public async Task<AuthenticationResponse?> CheckAndUpdateRefreshTokenAsync(string refreshToken)
        {
            ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(u=> u.RefreshToken == refreshToken);
            if(user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }
            return await GetAccessToken(user);
        }


        private string GenerateRefreshToken()
        {
            Byte[] bytes = new Byte[64];
            var random = RandomNumberGenerator.Create();

            random.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        private async Task RecordRefreshToken(string refreshToken, ApplicationUser user)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["RefreshToken:Expiration_days"]));
            await _userManager.UpdateAsync(user);
        }

        //Profile
        public async Task<ProfileDto?> GetProfile(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            if(user == null) return null;
            return _mapper.Map<ProfileDto>(user);
        }
    }
}
