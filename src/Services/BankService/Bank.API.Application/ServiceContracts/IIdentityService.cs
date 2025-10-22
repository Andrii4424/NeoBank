using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.DTOs.Users.Vacancies;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
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
        public Task LogoutAsync(ApplicationUser? user);
        public Task<bool> IsEmailUniqueAsync(string email);
        public Task<ApplicationUser> GetUserByEmailAsync(string email);
        public Task<AuthenticationResponse> GetAccessToken(ApplicationUser user);
        public Task<AuthenticationResponse?> CheckAndUpdateRefreshTokenAsync(string refreshToken);
        public Task<ProfileDto?> GetProfile(string id);
        public Task<OperationResult> UpdateProfile(ProfileDto profile);
        public Task<OperationResult> ApplyForJobAsync(string userId, VacancyDto vacancy);
        public Task<PageResult<ProfileDto>> GetEmployeesAsync(UserFilter filters);
        public Task<PageResult<ProfileDto>> GetUsersAsync(UserFilter filters);
    }
}
