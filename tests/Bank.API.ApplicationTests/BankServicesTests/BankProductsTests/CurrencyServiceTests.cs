using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Services.BankServices.BankProducts;
using Bank.API.Domain.Entities;
using Bank.API.Domain.RepositoryContracts;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.API.ApplicationTests.BankServicesTests.BankProductsTests
{
    public class CurrencyServiceTests
    {
        private readonly Mock<ILogger<CurrencyService>> _loggerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<IBankRepository> _bankRepositoryMock;
        private readonly CurrencyService _service;

        public CurrencyServiceTests()
        {
            _loggerMock = new Mock<ILogger<CurrencyService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _bankRepositoryMock = new Mock<IBankRepository>();

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new List<CurrencyDto>
                    {
                        new CurrencyDto { cc = "USD", rate = 40 },
                        new CurrencyDto { cc = "EUR", rate = 42 },
                        new CurrencyDto { cc = "PLN", rate = 10 }
                    })
                });

            var httpClient = new HttpClient(handlerMock.Object);

            _bankRepositoryMock
                .Setup(r => r.GetValueByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new BankEntity
                {
                    PercentageCommissionForBuyingCurrency = 5,
                    PercentageCommissionForSellingCurrency = 5
                });

            _service = new CurrencyService(
                _memoryCache,
                _bankRepositoryMock.Object,
                _loggerMock.Object,
                httpClient
            );
        }

        [Fact]
        public async Task GetCurrencyData_ShouldReturnOnlyUsdAndEur()
        {
            // Act
            var result = await _service.GetCurrencyData();

            // Assert
            result.Should().HaveCount(2);
            result.Any(c => c.cc == "USD").Should().BeTrue();
            result.Any(c => c.cc == "EUR").Should().BeTrue();
        }
    }
}