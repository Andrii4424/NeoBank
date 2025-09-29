using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices.BankProducts
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";

        public CurrencyService() { }

        public async Task<string> GetCurrencyData()
        {
            using var client = new HttpClient();
            return await client.GetStringAsync(url);

        }
    }
}
