using AutoMapper;
using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Auth;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.RepositoryContracts.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.Auth
{
    public class RecoveryPasswordService : IRecoveryPasswordService
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RecoveryPasswordService> _logger;
        private readonly ISmtpService _smtpService;


        public RecoveryPasswordService(UserManager<ApplicationUser> userManager, ILogger<RecoveryPasswordService> logger, ISmtpService smtpService,
            IIdentityService identityService)
        {
            _userManager = userManager;
            _logger = logger;
            _smtpService = smtpService;
            _identityService = identityService;
        }

        public async Task<OperationResult> SetAndSendRefreshPasswordCodeAsync(string userEmail)
        {
            _logger.LogInformation("User trying {email} change password", userEmail);
            try
            {
                ApplicationUser user = await _identityService.GetUserByEmailAsync(userEmail);

                var bytes = new byte[4];
                RandomNumberGenerator.Fill(bytes);
                var value = BitConverter.ToUInt32(bytes, 0) % 900000 + 100000;
                user.RefreshCode = value.ToString();
                user.RefreshCodeExpiryTime = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);

                await _smtpService.SendAsync(userEmail, "NeoBank – password recovery", $"<h3>Your password reset code:</h3><p><b>{user.RefreshCode}</b></p>");

                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error try change user {email} password. {errorMessage}", userEmail, ex.Message);

                return OperationResult.Error(ex.Message);
            }
        }

        public async Task<bool> ValidateRefreshPasswordCodeAsync(string userEmail, string code)
        {
            _logger.LogInformation("Validating refresh password code for user {userEmail}", userEmail);
            try
            {
                ApplicationUser user = await _identityService.GetUserByEmailAsync(userEmail);
                if (code == user.RefreshCode && user.RefreshCodeExpiryTime > DateTime.UtcNow)
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
                ApplicationUser user = await _identityService.GetUserByEmailAsync(changePasswordDetails.Email);
                if (changePasswordDetails.RefreshCode == user.RefreshCode)
                {
                    await _userManager.RemovePasswordAsync(user);
                    IdentityResult result = await _userManager.AddPasswordAsync(user, changePasswordDetails.NewPassword);
                    if (!result.Succeeded)
                    {
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
                ApplicationUser user = await _identityService.GetUserByEmailAsync(email);
                user.RefreshCode = null;
                user.RefreshCodeExpiryTime = null;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Success delete refresh code and date for user: {email}", email);

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed delete refresh code and date for user: {email}. {errorMessage}", email, ex.Message);
            }
        }

    }
}
