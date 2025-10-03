using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.Enums.CardEnums;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Domain.RepositoryContracts.Users;
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

        public UserCardService(IUserCardsRepository userCardsRepository, IMapper mapper, ICardTariffsRepository cardTariffsRepository)
        {
            _userCardsRepository = userCardsRepository;
            _mapper = mapper;
            _cardTariffsRepository = cardTariffsRepository;
        }

        //Read
        public async Task<PageResult<UserCardsDto>?> GetUserCardsAsync(Guid userId, UserCardsFilter? filters)
        {
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

            return pageResult;
        }

        public async Task<UserCardsDto?> GetCardByIdAsync(Guid cardId)
        {
            await ExpirationStatusCheckerAsync(null, cardId);
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if(card == null)
            {
                return null;
            }
            UserCardsDto userCard = _mapper.Map<UserCardsDto>(card);
            userCard.CardTariffs = _mapper.Map<CardTariffsDto>(await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId));
            return userCard;
        }

        //Create
        public async Task<OperationResult> CreateCardAsync(CreateUserCardDto cardParams)
        {
            if(await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId) == null)
            {
                return OperationResult.Error("Card tariffs not found");
            }
            if(cardParams.UserId == null)
            {
                return OperationResult.Error("User is not found");
            }
            if (!await _userCardsRepository.IsCardUnique(cardParams.UserId.Value, cardParams.CardTariffId, cardParams.ChosenCurrency))
            {
                return OperationResult.Error("Youe already has this card in the chosen currency");
            }

            //Generate personal card information
            string? cardNumber = await GenerateCardNumber((await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId))!.BIN);
            if(cardNumber == null)
            {
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

            return OperationResult.Ok();
        }

        //Update
        public async Task<OperationResult> UpdateCardStatusAsync(ChangeStatusDto newStatusParams)
        {
            await ExpirationStatusCheckerAsync(null, newStatusParams.CardId);
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(newStatusParams.CardId);
            if(card == null)
            {
                return OperationResult.Error("Card not found");
            }
            if (card.Status == CardStatus.Expired)
            {
                return OperationResult.Error("The card has expired. The card has been blocked for withdrawals, the card cannot be blocked/unblocked at this time. " +
                    "To remove the expired status, we recommend reissuing the card.");
            }

            card.Status = newStatusParams.NewStatus;
            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateCardPinAsync(Guid cardId, string newPin)
        {
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                return OperationResult.Error("Card not found");
            }
            card.Pin = newPin;
            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateCardBalanceAsync(Guid cardId, decimal amount)
        {
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                return OperationResult.Error("Card not found");
            }
            if(card.Status != CardStatus.Active)
            {
                return OperationResult.Error("Card is not active");
            }
            if(card.Balance + card.CreditLimit + amount < 0)
            {
                return OperationResult.Error("Insufficient funds");
            }
            card.Balance += amount;

            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateCardCreditLimitAsync(Guid cardId, decimal newCreditLimit)
        {
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                return OperationResult.Error("Card not found");
            }

            CardTariffsEntity? cardTariff = await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId);
            if (cardTariff.Type == CardType.Debit)
            {
                return OperationResult.Error("Debit card cannot have credit limit");
            }
            if(cardTariff.MaxCreditLimit< newCreditLimit)
            {
                return OperationResult.Error($"Max limit is {cardTariff.MaxCreditLimit} for this card");
            }
            if (newCreditLimit < 0)
            {
                return OperationResult.Error("Credit limit cannot be negative");
            }
            if(newCreditLimit + card.Balance < 0)
            {
                return OperationResult.Error("Total balance (balance + credit limit) cannot be negative.");
            }

            card.CreditLimit = newCreditLimit;
            _userCardsRepository.UpdateObject(card);
            await _userCardsRepository.SaveAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ReissueCardAcync(Guid cardId)
        {
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null) {
                return OperationResult.Error("Card not found");
            }

            CardTariffsEntity? cardTariffs = await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId);
            if (cardTariffs == null) {
                throw new NullReferenceException("Card Tariffs not found");
            }

            string? cardNumber = await GenerateCardNumber((await _cardTariffsRepository.GetValueByIdAsync(card.CardTariffId))!.BIN);
            if (cardNumber == null)
            {
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

            return OperationResult.Ok();
        }

        //Delete
        public async Task<OperationResult> DeleteCardAsync(Guid cardId)
        {
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if (card == null)
            {
                return OperationResult.Error("Card not found");
            }
            else if (card.Balance < 0)
            {
                return OperationResult.Error("Card balance must be 0 to close the card. Please top up your balance");
            }
            else if (card.Balance > 0)
            {
                return OperationResult.Error("Card balance must be 0 to close the card. Please transfer funds to another card");
            }
            _userCardsRepository.DeleteElement(card);
            await _userCardsRepository.SaveAsync();
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
    }
}
