using Bank.API.Domain.Abstractions;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.Entities.Cards
{
    public class UserCardsEntity : IHasId
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ApplicationUser User { get; private set; }
        public Guid UserId { get; set; }

        public CardTariffsEntity CardTariff { get; private set; }
        public Guid CardTariffId { get; set; }

        [StringLength(16)]
        public string CardNumber { get; set; }
        public DateOnly ExpiryDate { get; set; }

        public PaymentSystem ChosedPaymentSystem { get; set; }
        public Currency ChosenCurrency { get; set; }

        [StringLength(4)]
        public string Pin { get; set; }
        public CardStatus Status { get; set; }
        [StringLength(3)]
        public string CVV { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal Balance { get; set; }

        public UserCardsEntity() { }

        public UserCardsEntity(Guid userId, Guid cardTariffId, string cardNumber, DateOnly expiryDate, PaymentSystem chosedPaymentSystem, 
            Currency chosenCurrency, string pin, string cvv)
        {
            UserId = userId;
            CardTariffId = cardTariffId;
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            ChosedPaymentSystem = chosedPaymentSystem;
            ChosenCurrency = chosenCurrency;
            Pin = pin;
            CVV = cvv;
            Status = CardStatus.Active;
            Balance = 0;
            CreditLimit = 0;
        }
    }
}
