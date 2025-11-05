using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Services.BankServices.BankProducts;
using Bank.API.Domain.Entities;
using Bank.API.Domain.RepositoryContracts;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.ApplicationTests.BankServicesTests.BankProductsTests
{
    public class CurrencyServiceTests
    {
        private readonly Mock<ILogger<CurrencyService>> _loggerMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<IBankRepository> _bankRepositoryMock;
        private readonly CurrencyService _service;
        public Func<HttpClient> HttpClientFactory { get; set; } = () => new HttpClient();

        public CurrencyServiceTests()
        {
            _loggerMock = new Mock<ILogger<CurrencyService>>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _bankRepositoryMock = new Mock<IBankRepository>();

            _service = new CurrencyService(
                _memoryCacheMock.Object,
                _bankRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        //TO DO: Add tests for GetCurrencyData method without caching

    }
}
