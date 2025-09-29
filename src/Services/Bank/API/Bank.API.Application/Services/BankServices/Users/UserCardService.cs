using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.Helpers.HelperClasses;
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

        public UserCardService(IUserCardsRepository userCardsRepository, IMapper mapper, ICardTariffsRepository cardTariffsRepository)
        {
            _userCardsRepository = userCardsRepository;
            _mapper = mapper;
            _cardTariffsRepository = cardTariffsRepository;
        }

        //Read
        public async Task<List<UserCardsDto>?> GetUserCardsAsync(Guid userId)
        {
            List<UserCardsDto>? userCards = _mapper.Map<List<UserCardsDto>>(await _userCardsRepository.GetUserCardsAsync(userId));

            List<CardTariffsDto> cardTariffs = _mapper.Map<List<CardTariffsDto>>(await _cardTariffsRepository.GetAllValuesAsync());
            foreach (UserCardsDto card in userCards)
            {
                card.CardTariffs = cardTariffs.FirstOrDefault(ct => ct.Id == card.CardTariffId);
            }
            return userCards;
        }

        //Create
        public async Task<OperationResult> CreateCardAsync(CreateUserCardDto cardParams)
        {
            if(await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId) == null)
            {
                return OperationResult.Error("Card tariffs not found");
            }
            if(!await _userCardsRepository.IsCardUnique(cardParams.UserId, cardParams.CardTariffId, cardParams.ChosenCurrency))
            {
                return OperationResult.Error("User already has this card in the chosen currency");
            }

            //Generate personal card information
            string? cardNumber = await GenerateCardNumber((await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId))!.BIN);
            if(cardNumber == null)
            {
                return OperationResult.Error("Could not generate unique card number, please try again");
            }

            double validityPeriod = (await _cardTariffsRepository.GetValueByIdAsync(cardParams.CardTariffId))!.ValidityPeriod;
            DateOnly expieryDate = DateOnly.FromDateTime(DateTime.Now.AddYears((int)validityPeriod%1));
            if(validityPeriod%1 == 0.5)
            {
                expieryDate = expieryDate.AddMonths(6);
            }
            expieryDate = new DateOnly(expieryDate.Year, expieryDate.Month, 1);


            string cvv = GenerateCVV();

            UserCardsEntity userCard = new UserCardsEntity(cardParams.UserId, cardParams.CardTariffId, cardNumber, expieryDate,
                cardParams.ChosenPaymentSystem, cardParams.ChosenCurrency, cardParams.Pin, cvv);

            await _userCardsRepository.AddAsync(userCard);
            await _userCardsRepository.SaveAsync();

            return OperationResult.Ok();
        }

        //Update
        public async Task<OperationResult> UpdateCardStatusAsync(Guid cardId, CardStatus status)
        {
            UserCardsEntity? card = await _userCardsRepository.GetValueByIdAsync(cardId);
            if(card == null)
            {
                return OperationResult.Error("Card not found");
            }

            card.Status = status;
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
    }
}
