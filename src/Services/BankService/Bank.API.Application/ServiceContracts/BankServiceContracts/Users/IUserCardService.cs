using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.Users
{
    public interface IUserCardService
    {
        public Task<PageResult<UserCardsDto>?> GetUserCardsAsync(Guid userId, UserCardsFilter? filters);
        public Task<UserCardsDto?> GetCardByIdAsync(Guid cardId);
        public Task<OperationResult> CreateCardAsync(CreateUserCardDto cardParams);
        public Task<OperationResult> UpdateCardStatusAsync(ChangeStatusDto newStatusParams);
        public Task<OperationResult> UpdateCardPinAsync(Guid cardId, string newPin);
        public Task<OperationResult> UpdateCardBalanceAsync(Guid cardId, decimal amount);
        public Task<OperationResult> UpdateCardCreditLimitAsync(Guid cardId, decimal newCreditLimit);
        public Task<OperationResult> DeleteCardAsync(Guid cardId);
        public Task<OperationResult> ReissueCardAcync(Guid cardId);
        public Task<TransactionDetailsDto> GetTransactionDetails(TransactionDetailsDto details);
        public Task<bool> IsEnoughMoney(CardOperationDto operationInfo);
        public Task<double> GetP2PComissionByUserCardId(Guid cardId);
        public Task<TransactionDto> UpdateBalanceAfterTransactionAsync(TransactionDto? transaction);
    }
}
