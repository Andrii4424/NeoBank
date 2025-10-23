using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.DTOs.Users.Cards;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Application.Services.BankServices.BankProducts;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.Enums.CardEnums;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Domain.RepositoryContracts.Users;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices.Users
{
    public class UserCardService : IUserCardService
    {
        private readonly IUserCardsRepository _userCardsRepository;
        private readonly ICardTariffsRepository _cardTariffsRepository;
        private readonly IMapper _mapper;
        private readonly int _pageSize = SharedMethods.GetDefaultPageSize();
        private readonly ILogger<UserCardService> _logger;


        public UserCardService(IUserCardsRepository userCardsRepository, IMapper mapper, ICardTariffsRepository cardTariffsRepository,
            ILogger<UserCardService> logger)
        {
            _userCardsRepository = userCardsRepository;
            _mapper = mapper;
            _cardTariffsRepository = cardTariffsRepository;
            _logger = logger;
        }

        //Read
        public async Task<PageResult<UserCardsDto>?> GetUserCardsAsync(Guid userId, UserCardsFilter? filters)
        {
            _logger.LogInformation("Trying to get user cards page, userId: {userId}", userId);
            await ExpirationStatusCheckerAsync(userId, null);
            filters.PageNumber = filters.PageNumber ?? 1;
            FiltersDto<UserCardsEntity> filtersDto = filters.ToGeneralFilters();
            List<UserCardsDto>? userCards = _mapper.Map<List<UserCardsDto>>(await 
                _userCardsRepository.GetUserCardsAsync(userId, filtersDto.PageNumber.Value, _pageSize, filtersDto.SearchFilter,
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters));

            List<CardTariffsDto> cardTariffs = _mapper.Map<List<CardTariffsDto>>(await _cardTariffsRepository.GetAllValuesAsync());
            foreach (UserCardsDto card in userCards)
            {
                card.CardTariffs = cardTariffs.FirstOrDefault(ct => ct.Id == card.CardTariffId);
            }

            PageResult<UserCardsDto> pageResult = new PageResult<UserCardsDto>(userCards,
                await _userCardsRepository.GetUserCardsCountAsync(userId, filtersDto.SearchFilter, filtersDto.Filters), filters.PageNumber.Value, _pageSize);

            _logger.LogInformation("User cards page successfully getted, userId: {userId}", userId);
            return pageResult;
        }

        public async Task<List<UserCardsDto>?> GetUnfiltredUserCardsAsync(Guid userId)
        {
            _logger.LogInformation("Getting user cards userId: {userId}", userId);
            await ExpirationStatusCheckerAsync(userId, null);

            List<UserCardsDto>? userCards = _mapper.Map<List<UserCardsDto>>(await _userCardsRepository.GetUnfiltredUserCardsAsync(userId));
            List<CardTariffsDto> cardTariffs = _mapper.Map<List<CardTariffsDto>>(await _cardTariffsRepository.GetAllValuesAsync());
            foreach (UserCardsDto card in userCards)
            {
                card.CardTariffs = cardTariffs.FirstOrDefault(ct => ct.Id == card.CardTariffId);
            }
            return userCards;
        }

        public async Task<List<CroppedUserCardsDto>?> GetUnfiltredCroppedUserCardsAsync(Guid userId)
        {
            _logger.LogInformation("Getting cropped user cards userId: {userId}", userId);

            List<CroppedUserCardsDto>? userCards = _mapper.Map<List<CroppedUserCardsDto>>(await _userCardsRepository.GetUnfiltredUserCardsAsync(userId));
            List<CardTariffsDto> cardTariffs = _mapper.Map<List<CardTariffsDto>>(await _cardTariffsRepository.GetAllValuesAsync());
            foreach (CroppedUserCardsDto card in userCards)
            {
                card.CardTariffs = cardTariffs.FirstOrDefault(ct => ct.Id == card.CardTariffId);
            }
            return userCards;
        }

        public async Task<UserCardsDto?> GetCardByIdAsync(Guid cardId)
        {
            _logger.LogInformation("Trying to get user card info, cardId: {cardId}", cardId);

            await ExpirationStatusCheckerAsync(null, cardId);
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if(card == null)
            {
                return null;
            }
            UserCardsDto userCard = _mapper.Map<UserCardsDto>(card);
            userCard.CardTariffs = _mapper.Map<CardTariffsDto>(await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId));

            _logger.LogInformation("User cards info successfully getted, cardId: {cardId}", cardId);
            return userCard;
        }

        public async Task<bool> IsEnoughMoney(CardOperationDto operationInfo)
        {
            _logger.LogInformation("Check is enougth money for card: {cardId}, amount: {amount}", operationInfo.cardId, operationInfo.amount);
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(operationInfo.cardId);
            if (card == null) {
                _logger.LogInformation("Failed checking is enougth money for card: {cardId}, card doesnt exist", operationInfo.cardId);
                throw new ArgumentException("Card not found");
            }

            _logger.LogInformation("Success checking is enougth money for card: {cardId}, amount: {amount}, result: {result}", operationInfo.cardId, 
                operationInfo.amount, operationInfo.amount <= card.Balance);
            return operationInfo.amount <= card.Balance;
        }

        public async Task<double> GetP2PComissionByUserCardId(Guid cardId)
        {
            _logger.LogInformation("Try get p2p comission for card: {cardId}", cardId);

            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                _logger.LogInformation("Failed getting p2p comission for card: {cardId}, card doesnt exist", cardId);
                throw new ArgumentException("Card not found");
            }
            _logger.LogInformation("Success getting p2p comission for card: {cardId}", cardId);
            return (await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId)).P2PInternalCommission;
        }

        public async Task<TransactionDetailsDto> GetTransactionDetails(TransactionDetailsDto details)
        {
            _logger.LogInformation("Trying to get transaction details");
            if (details.SenderCardId != null)
            {
                UserCardsEntity? senderCard = await _userCardsRepository.GetValueByIdAsync(details.SenderCardId.Value);
                if (senderCard == null)
                {
                    _logger.LogError("Failed when getting transaction details. Sender card id didnt found, {senderCardId}", details.SenderCardId);
                    throw new ArgumentException("Sender card id didnt found");
                }
                details.SenderCurrency = senderCard.ChosenCurrency;

                details.SenderId = await GetUserIdByCardAsync(details.SenderCardId.Value);
                if (details.SenderId == null) { 
                    _logger.LogError("Failed when getting transaction details. Sender didnt found {senderCardId}", details.SenderCardId);
                    throw new NullReferenceException("Sender didnt found");
                }
            }
            if (details.GetterCardId != null)
            {
                UserCardsEntity? getterCard = await _userCardsRepository.GetValueByIdAsync(details.GetterCardId.Value);
                if (getterCard == null)
                {
                    _logger.LogError("Failed when getting transaction details. Getter card id didnt found, {getterCardId}", details.GetterCardId);
                    throw new ArgumentException("Sender card id didnt found");
                }
                details.GetterCurrency = getterCard.ChosenCurrency;

                details.GetterId = await GetUserIdByCardAsync(details.GetterCardId.Value);
                if (details.GetterId == null) { 
                    _logger.LogError("Failed when getting transaction details. Reciver Card didnt found {getterCardId}", details.GetterCardId);
                    throw new NullReferenceException("Reciver Card didnt found");
                }
            }
            _logger.LogInformation("Successfully getting transaction details");
            return details;
        }

        public async Task<Guid?> GetCardIdByCardNumberAsync(string cardNumber)
        {
            _logger.LogInformation("Getting card Id by card number");
            return await _userCardsRepository.GetCardIdByCardNumberAsync(cardNumber);
        }

        //Create
        public async Task<OperationResult> CreateCardAsync(CreateUserCardDto cardParams)
        {
            _logger.LogInformation("Trying to create user card for user {userId}", cardParams.UserId);
            if (await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId) == null)
            {
                _logger.LogError("Failed creating user card for user {userId}. Card tariffs not found", cardParams.UserId);
                return OperationResult.Error("Card tariffs not found");
            }
            if(cardParams.UserId == null)
            {
                _logger.LogError("Failed creating user card for user {userId}. User is not found", cardParams.UserId);
                return OperationResult.Error("User is not found");
            }
            if (!await _userCardsRepository.IsCardUnique(cardParams.UserId.Value, cardParams.CardTariffId, cardParams.ChosenCurrency))
            {
                _logger.LogError("Failed creating user card for user {userId}. User already has this card in the chosen currency", cardParams.UserId);
                return OperationResult.Error("You already has this card in the chosen currency");
            }

            //Generate personal card information
            string? cardNumber = await GenerateCardNumber((await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId))!.BIN);
            if(cardNumber == null)
            {
                _logger.LogError("Failed creating user card for user {userId}. Could not generate unique card number", cardParams.UserId);
                return OperationResult.Error("Could not generate unique card number, please try again");
            }

            double validityPeriod = (await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId))!.ValidityPeriod;
            DateOnly expieryDate = DateOnly.FromDateTime(DateTime.Now.AddYears((int)validityPeriod/1));
            if(validityPeriod%1 == 0.5)
            {
                expieryDate = expieryDate.AddMonths(6);
            }
            expieryDate = new DateOnly(expieryDate.Year, expieryDate.Month, 1);


            string cvv = GenerateCVV();

            UserCardsEntity userCard = new UserCardsEntity(cardParams.UserId.Value, cardParams.CardTariffId, cardNumber, expieryDate,
                cardParams.ChosenPaymentSystem, cardParams.ChosenCurrency, cardParams.Pin, cvv);

            await _userCardsRepository.AddAsync(userCard);
            await _userCardsRepository.SaveAsync();

            _logger.LogInformation("User card successfully created for user {userId}", cardParams.UserId);
            return OperationResult.Ok();
        }

        //Update
        public async Task<OperationResult> UpdateCardStatusAsync(ChangeStatusDto newStatusParams)
        {
            _logger.LogInformation("Trying update status for card {cardId}", newStatusParams.CardId);
            await ExpirationStatusCheckerAsync(null, newStatusParams.CardId);
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(newStatusParams.CardId);
            if(card == null)
            {
                _logger.LogInformation("Error update status for card {cardId}. Card not found", newStatusParams.CardId);
                return OperationResult.Error("Card not found");
            }
            if (card.Status == CardStatus.Expired)
            {
                _logger.LogInformation("Error update status for card {cardId}. The card has expired. The card has been blocked for withdrawals, " +
                    "the card cannot be blocked/unblocked at this time.To remove the expired status, we recommend reissuing the card.", newStatusParams.CardId);
                return OperationResult.Error("The card has expired. The card has been blocked for withdrawals, the card cannot be blocked/unblocked at this time. " +
                    "To remove the expired status, we recommend reissuing the card.");
            }

            card.Status = newStatusParams.NewStatus;
            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();

            _logger.LogInformation("Successgfully updated status for card {cardId}", newStatusParams.CardId);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateCardPinAsync(Guid cardId, string newPin)
        {
            _logger.LogInformation("Trying update pin for card {cardId}", cardId);
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                _logger.LogInformation("Error while updating pin for card {cardId}. Card not found", cardId);
                return OperationResult.Error("Card not found");
            }
            card.Pin = newPin;
            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();

            _logger.LogInformation("Success updating pin for card {cardId}", cardId);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateCardBalanceAsync(Guid cardId, decimal amount)
        {
            _logger.LogInformation("Trying update balance for card {cardId}", cardId);
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                _logger.LogInformation("Error while updating balance for card {cardId}. Card not found", cardId);
                return OperationResult.Error("Card not found");
            }
            if(card.Status != CardStatus.Active && amount<0)
            {
                _logger.LogInformation("Error while updating balance for card {cardId}. Card is not active", cardId);
                return OperationResult.Error("Card is not active");
            }
            if(card.Balance + card.CreditLimit + amount < 0)
            {
                _logger.LogInformation("Error while updating balance for card {cardId}. Insufficient funds", cardId);
                return OperationResult.Error("Insufficient funds");
            }
            card.Balance += amount;

            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();

            _logger.LogInformation("Success update balance for card {cardId}", cardId);
            return OperationResult.Ok();
        }

        public async Task<TransactionDto> UpdateBalanceAfterTransactionAsync(TransactionDto? transaction)
        {
            _logger.LogInformation("Trying update balance for cards");
            if (transaction ==null)
            {
                _logger.LogError("Error update balance for cards. Error with sending or getting transaction info: transaction is null");
                throw new ArgumentNullException("Error with sending or getting transaction info: transaction is null");
            }
            if (transaction.SenderCardId !=null)
            {
                if (transaction.AmountToWithdrawn == null)
                {
                    _logger.LogError("Error update balance for cards. Error with handling transaction, sender card specified, amount to withdrawn unspecified");
                    transaction.Success = false;
                    return transaction;
                }
                if (!(await UpdateCardBalanceAsync(transaction.SenderCardId.Value, (decimal)transaction.AmountToWithdrawn)).Success)
                {
                    transaction.Success = false;
                    return transaction;
                }
            }
            if (transaction.GetterCardId != null)
            {
                if (transaction.AmountToReplenish == null)
                {
                    _logger.LogError("Error update balance for cards. Error with handling transaction, getter card specified, amount to repleish unspecified");
                    transaction.Success = false;
                    return transaction;
                }
                if (!(await UpdateCardBalanceAsync(transaction.GetterCardId.Value, (decimal)transaction.AmountToReplenish)).Success)
                {
                    _logger.LogError("Error update balance for cards. Influent funds");
                    transaction.Success = false;
                    return transaction;
                }
            }
            transaction.Success = true;

            _logger.LogInformation("Success update balance for cards");
            return transaction;
        }

        public async Task<OperationResult> UpdateCardCreditLimitAsync(Guid cardId, decimal newCreditLimit)
        {
            _logger.LogInformation("Trying update credit limit for {cardId}", cardId);

            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                _logger.LogError("Failed update credit limit for {cardId}. Card not found", cardId);
                return OperationResult.Error("Card not found");
            }

            CardTariffsEntity? cardTariff = await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId);
            if (cardTariff.Type == CardType.Debit)
            {
                _logger.LogError("Failed update credit limit for {cardId}. Debit card cannot have credit limit", cardId);
                return OperationResult.Error("Debit card cannot have credit limit");
            }
            if(cardTariff.MaxCreditLimit< newCreditLimit)
            {
                _logger.LogError("Failed update credit limit for {cardId}. Max limit is {cardTariff.MaxCreditLimit} for this card", cardId, cardTariff.MaxCreditLimit);
                return OperationResult.Error($"Max limit is {cardTariff.MaxCreditLimit} for this card");
            }
            if (newCreditLimit < 0)
            {
                _logger.LogError("Failed update credit limit for {cardId}. Credit limit cannot be negative", cardId);
                return OperationResult.Error("Credit limit cannot be negative");
            }
            if(newCreditLimit + card.Balance < 0)
            {
                _logger.LogError("Failed update credit limit for {cardId}.Total balance (balance + credit limit) cannot be negative.", cardId);
                return OperationResult.Error("Total balance (balance + credit limit) cannot be negative.");
            }

            card.CreditLimit = newCreditLimit;
            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();

            _logger.LogInformation("Success update credit limit for {cardId}", cardId);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ReissueCardAcync(Guid cardId)
        {
            _logger.LogInformation("Trying reissue card for {cardId}", cardId);

            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null) {
                _logger.LogError("Error reissue card for {cardId}. Card not found", cardId);
                return OperationResult.Error("Card not found");
            }

            CardTariffsEntity? cardTariffs = await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId);
            if (cardTariffs == null) {
                _logger.LogError("Error reissue card for {cardId}. Card Tariffs not found", cardId);
                throw new NullReferenceException("Card Tariffs not found");
            }

            string? cardNumber = await GenerateCardNumber((await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId))!.BIN);
            if (cardNumber == null)
            {
                _logger.LogError("Error reissue card for {cardId}. Could not generate unique card number, please try again", cardId);
                return OperationResult.Error("Could not generate unique card number, please try again");
            }

            double validityPeriod = cardTariffs.ValidityPeriod;
            DateOnly expieryDate = DateOnly.FromDateTime(DateTime.Now.AddYears((int)validityPeriod / 1));
            if (validityPeriod % 1 == 0.5)
            {
                expieryDate = expieryDate.AddMonths(6);
            }
            expieryDate = new DateOnly(expieryDate.Year, expieryDate.Month, 1);
            string cvv = GenerateCVV();

            card.CVV = cvv;
            card.CardNumber = cardNumber;
            card.ExpiryDate = expieryDate;
            card.Status = CardStatus.Active;

            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();

            _logger.LogInformation("Success reissue card for {cardId}", cardId);
            return OperationResult.Ok();
        }

        //Delete
        public async Task<OperationResult> DeleteCardAsync(Guid cardId)
        {
            _logger.LogInformation("Trying delete card {cardId}", cardId);

            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                _logger.LogError("Failed deleting card {cardId}. Card not found", cardId);
                return OperationResult.Error("Card not found");
            }
            else if (card.Balance < 0)
            {
                _logger.LogError("Failed deleting card {cardId}. Card balance must be 0 to close the card. Please top up your balance", cardId);
                return OperationResult.Error("Card balance must be 0 to close the card. Please top up your balance");
            }
            else if (card.Balance > 0)
            {
                _logger.LogError("Failed deleting card {cardId}. Card balance must be 0 to close the card. Card balance must be 0 to close the card. " +
                    "Please transfer funds to another card", cardId);
                return OperationResult.Error("Card balance must be 0 to close the card. Please transfer funds to another card");
            }
            _userCardsRepository.DeleteElement(card);
            await _userCardsRepository.SaveAsync();

            _logger.LogInformation("Success delete card {cardId}", cardId);
            return OperationResult.Ok();
        }

        //Helpers
        private async Task<string?> GenerateCardNumber(string BIN)
        {
            for (int attemp=0; attemp< 1000; attemp++ )
            {
                Random random = new Random();
                string cardNumber = BIN;
                for (int i = 0; i < 10; i++)
                {
                    cardNumber += random.Next(0, 10).ToString();
                }
                
                if (await _userCardsRepository.IsCardNumberUnique(cardNumber))
                {
                    return cardNumber;
                }
            }     
            return null;
        }

        private string GenerateCVV()
        {
            Random random = new Random();
            string cvv = "";
            for (int i = 0; i < 3; i++)
            {
                cvv += random.Next(0, 10).ToString();
            }
            return cvv;
        }

        private async Task ExpirationStatusCheckerAsync(Guid? userId, Guid? cardId)
        {
            if (cardId != null)
            {
                UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId.Value);
                if (card != null && card.ExpiryDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    await ChangeStatusToExpired(card);
                }
            }
            else if (userId != null) { 
                List<UserCardsEntity> cards= await _userCardsRepository.GetAllExpiredUserCardsAsync(userId.Value);
                foreach (UserCardsEntity card in cards) {
                    await ChangeStatusToExpired(card);
                }
            }
        }
        
        private async Task ChangeStatusToExpired(UserCardsEntity? card)
        {
            card.Status = CardStatus.Expired;
            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();
        }

        private async Task<Guid?> GetUserIdByCardAsync(Guid cardId)
        {
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if(card ==null) return null;
            return card.UserId;
        }
    }
}
