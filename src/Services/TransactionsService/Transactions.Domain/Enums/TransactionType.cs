using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Domain.Enums
{
    public enum TransactionType
    {
        P2P = 0,
        Credit = 1,
        Deposit = 2,
        AmnualCardMatinance = 3,
        CurrencyExchange = 4
    }
}
