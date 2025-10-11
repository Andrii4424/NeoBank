using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Enums;

namespace Transactions.Application.DTOs
{
    public class ExchangeCurrencyDto
    {
        public Currency? From { get; set; }
        public Currency? To { get; set; }
        public double Amount { get; set; }
    }
}
