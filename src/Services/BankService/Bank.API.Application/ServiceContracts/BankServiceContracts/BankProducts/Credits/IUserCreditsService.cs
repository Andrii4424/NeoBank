using Bank.API.Application.DTOs.Credits;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts.Credits
{
    public interface IUserCreditsService
    {
        public Task<PageResult<UserCreditDto>> GetUserCredits(UserCreditsFilter filtersDto, Guid userId);
        public Task<TransactionDto> OpenUserCredit(TransactionDto transaction);
        public Task<TransactionDto> PayForCredit(TransactionDto transaction);
    }
}
