using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.DTOs.Users.Vacancies;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Auth;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.RepositoryContracts.Users;
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

namespace Bank.API.Application.Services.Auth
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<IdentityService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ISmtpService _smtpService;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, 
            IMapper mapper, IWebHostEnvironment env, ILogger<IdentityService> logger, IUserRepository userRepository, ISmtpService smtpService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _env = env;
            _logger = logger;
            _userRepository = userRepository;
            _smtpService = smtpService;
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

        public async Task<OperationResult> SetAndSendRefreshPasswordCodeAsync(string userEmail)
        {
            _logger.LogInformation("User trying {email} change password", userEmail);
            try
            {
                ApplicationUser user = await GetUserByEmailAsync(userEmail);

                var bytes = new byte[4];
                RandomNumberGenerator.Fill(bytes);
                var value = BitConverter.ToUInt32(bytes, 0) % 900000 + 100000;
                user.RefreshCode = value.ToString();
                user.RefreshCodeExpiryTime = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);               

                await _smtpService.SendAsync(userEmail, "NeoBank – password recovery", $"<h3>Your password reset code:</h3><p><b>{user.RefreshCode}</b></p>");

                return OperationResult.Ok();
            }
            catch (Exception ex) {
                _logger.LogInformation("Error try change user {email} password. {errorMessage}", userEmail, ex.Message);

                return OperationResult.Error(ex.Message);
            }
        }

        public async Task<bool> ValidateRefreshPasswordCodeAsync(string userEmail, string code)
        {
            _logger.LogInformation("Validating refresh password code for user {userEmail}", userEmail);
            try
            {
                ApplicationUser user = await GetUserByEmailAsync(userEmail);
                if(code == user.RefreshCode && user.RefreshCodeExpiryTime>DateTime.UtcNow)
                {
                    _logger.LogInformation("Success Validating refresh password code for user {userEmail}", userEmail);
                    return true;
                }
                else if (user.RefreshCodeExpiryTime < DateTime.UtcNow)
                {
                    await DeleteRefreshPasswordCodeAsync(userEmail);
                }
                _logger.LogInformation("Failed Validating refresh password code for user {userEmail}", userEmail);
                return false;
            }
            catch 
            {
                _logger.LogInformation("Failed Validating refresh password code for user {userEmail}", userEmail);
                return false;
            }
        }

        public async Task<OperationResult> UpdatePasswordAsync(ChangePasswordDto changePasswordDetails)
        {
            _logger.LogInformation("Changing password for user {userEmail}", changePasswordDetails.Email);

            try
            {
                ApplicationUser user = await GetUserByEmailAsync(changePasswordDetails.Email);
                if (changePasswordDetails.RefreshCode == user.RefreshCode)
                {
                    await _userManager.RemovePasswordAsync(user);
                    IdentityResult result = await _userManager.AddPasswordAsync(user, changePasswordDetails.NewPassword);
                    if (!result.Succeeded) { 
                        _logger.LogInformation("Failed changing password for user {userEmail}. Password is not safe", changePasswordDetails.Email);
                        return OperationResult.Error(result.Errors.First().Description);
                    }
                    await DeleteRefreshPasswordCodeAsync(changePasswordDetails.Email);
                    _logger.LogInformation("Success changing password for user {userEmail}", changePasswordDetails.Email);
                    return OperationResult.Ok();

                }
                _logger.LogInformation("Failed changing password for user {userEmail}. Refresh code is not valid, please try again", changePasswordDetails.Email);

                return OperationResult.Error("Refresh code is not valid, please try again");

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed changing password for user {userEmail}. {errorMessage}", changePasswordDetails.Email, ex.Message);

                return OperationResult.Error(ex.Message);
            }
        }

        private async Task DeleteRefreshPasswordCodeAsync(string email)
        {
            _logger.LogInformation("Delete refresh code and date for user: {email}", email);
            try
            {
                ApplicationUser user = await GetUserByEmailAsync(email);
                user.RefreshCode = null;
                user.RefreshCodeExpiryTime = null;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Success delete refresh code and date for user: {email}", email);

            }
            catch (Exception ex){ 
                _logger.LogInformation("Failed delete refresh code and date for user: {email}. {errorMessage}", email, ex.Message);
            }
        }

        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
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
        public async Task<ProfileDto?> GetFullProfile(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            if(user == null) return null;
            ProfileDto profile = _mapper.Map<ProfileDto>(user);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            profile.Role = roles.Contains("Admin") ? "Admin" : "User";
            return profile;
        }

        public async Task<CroppedProfileDto?> GetCroppedProfile(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;
            CroppedProfileDto profile = _mapper.Map<CroppedProfileDto>(user);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            profile.Role = roles.Contains("Admin") ? "Admin" : "User";
            return profile;
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

        //Get users
        public async Task<PageResult<ProfileDto>> GetUsersAsync(UserFilter filters)
        {
            _logger.LogInformation("Getting users page");

            IQueryable<ApplicationUser> query = _userManager.Users;

            return await GetUsersPageAsync(filters, query, false);
        }

        public async Task<PageResult<ProfileDto>> GetEmployeesAsync(UserFilter filters)
        {
            _logger.LogInformation("Getting employees page");
            IQueryable<ApplicationUser> query = _userManager.Users;

            IList<ApplicationUser> admins = await _userManager.GetUsersInRoleAsync("Admin");

            IEnumerable<Guid> adminsId = admins.Select(u => u.Id);

            query = query.Where(u => adminsId.Contains(u.Id));


            return await GetUsersPageAsync(filters, query, true);
        }

        private async Task<PageResult<ProfileDto>> GetUsersPageAsync(UserFilter filters, IQueryable<ApplicationUser> query, bool onlyAdmins)
        {
            filters.PageNumber = filters.PageNumber ?? 1;
            FiltersDto<ApplicationUser> filtersDto = filters.ToGeneralFilters();

            if (filtersDto.SearchFilter != null) query = query.Where(filtersDto.SearchFilter);
            if (filtersDto.Filters != null)
            {
                filtersDto.Filters = filtersDto.Filters.Where(v => v != null).ToList();
                foreach (var filter in filtersDto.Filters)
                {
                    query = query.Where(filter);
                }
            }
            if (filtersDto.SortValue != null)
            {
                query = filtersDto.Ascending ? query.OrderBy(filtersDto.SortValue).ThenBy(obj => obj.Id) :
                    query.OrderByDescending(filtersDto.SortValue).ThenBy(obj => obj.Id);
            }
            else
            {
                query = query.OrderBy(obj => obj.Id);
            }

            List<ApplicationUser> users = await query
                .Skip((filtersDto.PageNumber.Value - 1) * filtersDto.PageSize.Value)
                .Take(filtersDto.PageSize.Value)
                .ToListAsync();

            int elementsCount = await query
                .CountAsync();

            List<ProfileDto> usersDto = _mapper.Map<List<ProfileDto>>(users);

            //Getting roles for each user
            if (!onlyAdmins)
            {
                Dictionary<Guid, List<string?>> roles = await _userRepository.GetRolesDictionaryAsync(users);

                foreach (ProfileDto user in usersDto)
                {
                    if (roles.TryGetValue(user.Id, out List<string?> userRoles) && userRoles != null)
                    {
                        Console.WriteLine($"{user.Id} Admin: {roles[user.Id].Contains("Admin")} User: {roles[user.Id].Contains("User")}");
                        if (roles[user.Id].Contains("Admin")) user.Role = "Admin";
                        else if (roles[user.Id].Contains("User"))
                        {
                            user.Role = "User";
                        }
                        else user.Role = "User";
                    }
                    else
                    {
                        user.Role = "User";
                    }
                }

            }
            else
            {
                foreach (ProfileDto user in usersDto)
                {
                    user.Role = "Admin";
                }
            }

            PageResult<ProfileDto> pageResult = new PageResult<ProfileDto>(usersDto, elementsCount,
                filters.PageNumber.Value, filters.PageSize.Value);

            return pageResult;
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
                    (user.Email != null || profile.Email != null) &&
                    (user.FirstName != null || profile.FirstName != null) &&
                    (user.Surname != null || profile.Surname != null) &&
                    (user.Patronymic != null || profile.Patronymic != null) &&
                    (user.TaxId != null || profile.TaxId != null) &&
                    (user.PhoneNumber != null || profile.PhoneNumber != null) &&
                    (user.DateOfBirth != null || profile.DateOfBirth != null);
            }

            return user;
        }
    }
}
