using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.DTOs.Users.Vacancies;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bank.API.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, 
            IMapper mapper, IWebHostEnvironment env, ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _env = env;
            _logger = logger;
        }

        //Auth
        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Trying to register user with email {email}", registerDto.Email);
            if (!await IsEmailUniqueAsync(registerDto.Email))
            {
                _logger.LogError("Failed registering user with email {email}. Email is already in use.", registerDto.Email);
                return IdentityResult.Failed(new IdentityError { Description = "Email is already in use." });
            }
            ApplicationUser user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded) {
                _logger.LogInformation("Success register user with email {email}", registerDto.Email);
                await _userManager.AddToRoleAsync(user, "User"); 
            }
            return result;
        }

        public async Task LogoutAsync(ApplicationUser? user)
        {
            if (user != null) { 
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
                _logger.LogInformation("Logout user with email: {email}", user.Email);
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("Trying to get user with email: {email}", email);
            if (IsEmailUniqueAsync(email).Result)
            {
                _logger.LogInformation("Failed getting user with email: {email}. User not found", email);

                throw new ArgumentException("User not found");
            }
            _logger.LogInformation("Success getting user with email: {email}", email);
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<AuthenticationResponse> GetAccessToken(ApplicationUser user)
        {
            _logger.LogInformation("Trying to get access token for user with email: {email}", user.Email);

            //Access Token
            var claim = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(user.Id)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(),ClaimValueTypes.Integer64)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claim.AddRange(userClaims.Where(c =>
                !string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase)));

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles.Distinct(StringComparer.OrdinalIgnoreCase))
                claim.Add(new Claim(ClaimTypes.Role, role));

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

            _logger.LogInformation("Success getting access token for user with email: {email}", user.Email);
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

        public async Task<OperationResult> UpdateProfile(ProfileDto profile)
        {
            _logger.LogInformation("Trying to update user profile with {email}", profile.Email);

            var allowedAvatarExtension = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
            ApplicationUser? user = await _userManager.FindByIdAsync(profile.Id.ToString());
            if (user == null)
            {
                _logger.LogError("Failed updating user profile with {email}. User doesnt exist", profile.Email);
                throw new ArgumentException($"User with id {profile.Id} doesnt exist");
            }
            else if (profile.Email == null)
            {
                _logger.LogError("Failed updating user profile with {email}. Email has to be provided", profile.Email);
                return OperationResult.Error("Email has to be provided");
            }
            else if (profile.DateOfBirth!=null && DateOnly.FromDateTime(DateTime.Today).Year-profile.DateOfBirth.Value.Year <14)
            {
                _logger.LogError("Failed updating user profile with {email}. User must be 14 years or older to use the NeoBank", profile.Email);
                return OperationResult.Error("You must be 14 years or older to use the NeoBank");
            }
            else if (profile.Email!= user.Email && !await IsEmailUniqueAsync(profile.Email))
            {
                _logger.LogError("Failed updating user profile with {email}. Email is already in use.", profile.Email);
                return OperationResult.Error("Email is already in use.");
            }

            await _userManager.SetUserNameAsync(user, profile.Email);
            user = MapProfile(profile, user);

            if (profile.Avatar != null) {
                //Delete old photo if exists
                if (profile.AvatarPath!=null)
                {
                    string deletePath = Path.Combine(_env.WebRootPath, profile.AvatarPath);
                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }

                //Add new photo
                var ext = Path.GetExtension(profile.Avatar.FileName).ToLowerInvariant();
                if (!allowedAvatarExtension.Contains(ext)) {
                    return OperationResult.Error("Invalid image format. Allowed: JPG/JPEG, PNG, WEBP");
                }
                string fileName = $"{profile.Id}{profile.Avatar.FileName.ToLower()}";
                string absolutePath =Path.Combine(_env.WebRootPath, "uploads", "profile-photos", fileName);
                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await profile.Avatar.CopyToAsync(stream);
                }
                profile.AvatarPath = $"uploads/profile-photos/{fileName}";
            }
            user = MapProfile(profile,user);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Success updating user profile with {email}", profile.Email);
            return OperationResult.Ok();
        }

        //Job
        public async Task<OperationResult> ApplyForJobAsync(string userId, VacancyDto vacancy)
        {
            _logger.LogInformation("Try apply for a job user {userId} for vacancy {vacancyId}", userId, vacancy.Id);
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) { 
                _logger.LogInformation("Failed applying user {userId}for a job for vacancy {vacancyId}. User doesnt exist", userId, vacancy.Id);
                return OperationResult.Error("User doesnt exist");
            }
            if (user.IsVerified !=true) {
                _logger.LogInformation("Failed applying user {userId}for a job for vacancy {vacancyId}. User is not verified", userId, vacancy.Id);
                return OperationResult.Error("You must be verified for apply to the job. Pleace update data in profile");
            }
            if (user.JobTitle != null) {
                _logger.LogInformation("Failed applying user {userId}for a job for vacancy {vacancyId}. User is already has job", userId, vacancy.Id);
                return OperationResult.Error("You already has a job. Contact bank admin if you want change job title");
            }

            IdentityResult result = await ChangeRoleToAdminAsync(userId);
            if (!result.Succeeded)
            {
                _logger.LogInformation("Failed applying user {userId}for a job for vacancy {vacancyId}. Error when changing role.", userId, vacancy.Id);
                return OperationResult.Error("Error when changing role. Contact bank administrators ");
            }
            user.JobTitle = vacancy.JobTitle;
            user.Salary = vacancy.Salary;
            user.JobCategory= vacancy.Category;
            await _userManager.UpdateAsync(user);

            return OperationResult.Ok();
        }

        private async Task<IdentityResult> ChangeRoleToAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return removeResult;
            }

            var addResult = await _userManager.AddToRoleAsync(user, "Admin");

            return addResult;
        }

        private ApplicationUser MapProfile(ProfileDto profile, ApplicationUser user)
        {
            user.AvatarPath = profile.AvatarPath;
            user.Email = profile.Email;
            user.FirstName = profile.FirstName;
            user.Surname = profile.Surname;
            user.Patronymic = profile.Patronymic;
            user.DateOfBirth = profile.DateOfBirth;
            user.TaxId = profile.TaxId;
            user.PhoneNumber = profile.PhoneNumber;
            
            if(user.IsVerified ==false || user.IsVerified == null)
            {

                user.IsVerified =
                    ((user.Email != null) || (profile.Email != null)) &&
                    ((user.FirstName != null) || (profile.FirstName != null)) &&
                    ((user.Surname != null) || (profile.Surname != null)) &&
                    ((user.Patronymic != null) || (profile.Patronymic != null)) &&
                    ((user.TaxId != null) || (profile.TaxId != null)) &&
                    ((user.PhoneNumber != null) || (profile.PhoneNumber != null)) &&
                    ((user.DateOfBirth != null) || (profile.DateOfBirth != null));
            }

            return user;
        }
    }
}
