using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users.Cards
{
    public class CroppedUserCardsDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CardTariffId { get; set; }

        public CardTariffsDto? CardTariffs { get; set; }

        public string CardNumber { get; set; }

        public PaymentSystem ChosedPaymentSystem { get; set; }

    }
}
