using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.Helpers.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.Auth
{
    public interface IRecoveryPasswordService
    {
        public Task<OperationResult> SetAndSendRefreshPasswordCodeAsync(string userEmail);
        public Task<bool> ValidateRefreshPasswordCodeAsync(string userEmail, string code);
        public Task<OperationResult> UpdatePasswordAsync(ChangePasswordDto changePasswordDetails);
    }
}
