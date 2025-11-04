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

        [Fact]
        public async Task GetCurrencyData_ShouldReturnFromCache_WhenCacheExists()
        {
            // Arrange
            var expectedData = new List<CurrencyDto>
            {
                new CurrencyDto { cc = "USD", rate = 38.5 },
                new CurrencyDto { cc = "EUR", rate = 41.2 }
            };

            object outObj = expectedData;
            _memoryCacheMock
                .Setup(c => c.TryGetValue(It.IsAny<object>(), out outObj))
                .Returns(true);

            // Act
            var result = await _service.GetCurrencyData();

            // Assert
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetCurrencyData_ShouldFetchFromApi_WhenCacheIsEmpty()
        {
            // Arrange
            object outObj = null!;
            _memoryCacheMock
                .Setup(c => c.TryGetValue(It.IsAny<object>(), out outObj))
                .Returns(false);

            var bank = new BankEntity
            {
                PercentageCommissionForBuyingCurrency = 2,
                PercentageCommissionForSellingCurrency = 3
            };
            _bankRepositoryMock.Setup(r => r.GetValueByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(bank);

            // Мокаем HttpClient через фейковый handler
            var fakeRates = new List<CurrencyDto>
            {
                new CurrencyDto { cc = "USD", rate = 38.123 },
                new CurrencyDto { cc = "EUR", rate = 41.987 },
                new CurrencyDto { cc = "GBP", rate = 46.5 } 
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(fakeRates)
                });

            var httpClient = new HttpClient(handlerMock.Object);

            // Act
            var result = await _service.GetCurrencyData();

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(r => r.cc == "USD" || r.cc == "EUR");

            var usd = result.First(r => r.cc == "USD");
            usd.NeoBankBuyCource.Should().Be(Math.Round(38.123 * 1.02, 2));
            usd.NeoBankSellCource.Should().Be(Math.Round(38.123 * 1.03, 2));
        }


    }
}
