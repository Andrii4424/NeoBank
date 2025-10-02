using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.DTOs.Users.CardOperations
{
    public class ExchangeCurrencyDto
    {
        public Currency From { get; set; }
        public Currency To { get; set; }
        public double Amount { get; set; }
    }
}
