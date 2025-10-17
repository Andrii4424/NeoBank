using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Bank.API.Domain.Entities;
using Bank.API.Domain.Enums.CardEnums;
using Bank.API.Domain.RepositoryContracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices.BankProducts
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
        private readonly IMemoryCache _memoryCache;
        private readonly string cacheKey = "CurrencyData";
        private readonly IBankRepository _bankRepository;
        private readonly ILogger<CurrencyService> _logger;


        public CurrencyService(IMemoryCache memoryCache, IBankRepository bankRepository, ILogger<CurrencyService> logger) {
            _memoryCache = memoryCache;
            _bankRepository = bankRepository;
            _logger = logger;
        }

        public async Task<List<CurrencyDto>> GetCurrencyData()
        {
            _logger.LogInformation("Trying to get currencies rates");
            if (_memoryCache.TryGetValue(cacheKey, out List<CurrencyDto> currencyRates))
            {
                _logger.LogInformation("Success getting currencies rates from cache");
                return currencyRates;
            }
            else
            {
                using var client = new HttpClient();
                var allRates = await client.GetFromJsonAsync<List<CurrencyDto>>(url);

                 allRates = allRates!
                    .Where(r => r.cc == "USD" || r.cc == "EUR")
                    .ToList();

                BankEntity bank = await _bankRepository.GetValueByIdAsync(SharedMethods.GetBankGuid());
                if(bank == null) {
                    throw new Exception("Bank not found");
                }
                foreach (var currencyRate in allRates) { 
                    currencyRate.rate = Math.Round(currencyRate.rate, 2);
                    currencyRate.NeoBankBuyCource = Math.Round(currencyRate.rate * (1 + bank.PercentageCommissionForBuyingCurrency / 100), 2);
                    currencyRate.NeoBankSellCource = Math.Round(currencyRate.rate * (1 + bank.PercentageCommissionForSellingCurrency / 100), 2);
                }

                var CachEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetPriority(CacheItemPriority.Normal);

                _memoryCache.Set(cacheKey, allRates, CachEntryOptions);

                _logger.LogInformation("Success getting currencies rates from request");
                return allRates;
            }
        }


        public async Task<decimal?> ExchangeCurrency(ExchangeCurrencyDto exchangeParams)
        {
            _logger.LogInformation("Getting exchanged currency");

            List<CurrencyDto> rates = await GetCurrencyData();
            Dictionary<Currency, double?> sellRates = new Dictionary<Currency, double?> {
                {Currency.UAH, 1 },
                {Currency.USD, rates.FirstOrDefault(c=>c.cc=="USD").NeoBankSellCource.Value},
                {Currency.EUR, rates.FirstOrDefault(c=>c.cc=="EUR").NeoBankSellCource.Value}
            };
            Dictionary<Currency, double?> buyRates = new Dictionary<Currency, double?> {
                {Currency.UAH, 1 },
                {Currency.USD, rates.FirstOrDefault(c=>c.cc=="USD").NeoBankBuyCource.Value},
                {Currency.EUR, rates.FirstOrDefault(c=>c.cc=="EUR").NeoBankBuyCource.Value}
            };
            double? fromCource = buyRates[exchangeParams.From]; 
            double? toCource = sellRates[exchangeParams.To];

            if(fromCource==null || toCource == null)
            {
                throw new NullReferenceException("Error with getting NBU currency cource");
            }

            //Double conversion
            if(exchangeParams.From != Currency.UAH && exchangeParams.To!= Currency.UAH)
            {
                exchangeParams.Amount = exchangeParams.Amount * fromCource.Value;
                fromCource = 1;
            }

            return Math.Round((decimal)(fromCource / toCource) * (decimal)exchangeParams.Amount, 2);
        }
    }
}
