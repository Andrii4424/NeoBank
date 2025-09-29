using Bank.API.Application.DTOs.BankProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts
{
    public interface ICurrencyService
    {
        public Task<List<CurrencyDto>> GetCurrencyData();

    }
}
