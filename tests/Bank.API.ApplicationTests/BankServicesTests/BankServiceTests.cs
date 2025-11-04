using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Services.BankServices;
using Bank.API.ApplicationTests.Helpers;
using Bank.API.Domain.Entities;
using Bank.API.Domain.RepositoryContracts;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.ApplicationTests.BankServicesTests
{
    public class BankServiceTests
    {
        private readonly Mock<IBankRepository> _bankRepositoryMock;
        private readonly IFixture _fixture;
        private readonly BankService _service;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<BankService>> _mockLogger;


        public BankServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _bankRepositoryMock = _fixture.Freeze<Mock<IBankRepository>>();
            _mockMapper = _fixture.Freeze<Mock<IMapper>>();
            _mockCache = _fixture.Freeze<Mock<IMemoryCache>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<BankService>>>();

            _service = new BankService(
                _bankRepositoryMock.Object,
                _mockMapper.Object,
                _mockCache.Object,
                _mockLogger.Object
            );
        }

        #region ReadBank
        [Fact] 
        public async Task GetBankInfo_WithNoArguments_ReturnsBankDto()
        {
            //Arrange
            BankEntity bank = BankHelper.GetDefaultBankEntity();
            BankDto bankDto = BankHelper.GetDefaultBankDto();

            _bankRepositoryMock
                .Setup(repo => repo.GetValueByIdAsync(BankHelper.GetBankId()))
                .ReturnsAsync(bank);

            _mockMapper
                .Setup(m => m.Map<BankDto>(bank))
                .Returns(bankDto);

            //Act
            var result = await _service.GetBankInfo();

            //Assert
            result.Should().BeEquivalentTo(bankDto);
        }

        #endregion

        #region UpdateBank
        [Fact]
        public async Task UpdateBank_WithNullArgument_ThrowsArgumentNullException()
        {
            //Arrange
            BankDto? bankDto = null;

            //Act
            Func<Task> result = async () => await _service.UpdateBank(bankDto);

            //Assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateBank_WithCorrectData_ReturnsSuccessOperationResult()
        {
            //Arrange
            BankDto? bankDto = BankHelper.GetDefaultBankDto();
            BankEntity bankEntity = BankHelper.GetDefaultBankEntity();

            _bankRepositoryMock
                .Setup(repo => repo.GetValueByIdAsync(BankHelper.GetBankId()))
                .ReturnsAsync(bankEntity);

            _bankRepositoryMock
                .Setup(repo => repo.UpdateObject(bankEntity));

            _bankRepositoryMock
                .Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map(bankDto, bankEntity));

            //Act
            OperationResult result = await _service.UpdateBank(bankDto);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        #endregion
    }
}
