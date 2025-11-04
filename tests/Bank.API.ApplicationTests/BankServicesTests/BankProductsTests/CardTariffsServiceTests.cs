using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.Services.BankServices;
using Bank.API.Application.Services.BankServices.BankProducts;
using Bank.API.ApplicationTests.Helpers;
using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.ApplicationTests.BankServicesTests.BankProductsTests
{
    public class CardTariffsServiceTests
    {
        private readonly Mock<ICardTariffsRepository> _cardTariffsMock;
        private readonly IFixture _fixture;
        private readonly CardTariffsService _service;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<CardTariffsService>> _mockLogger;


        public CardTariffsServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customize<DateOnly>(c => c.FromFactory(() => DateOnly.FromDateTime(DateTime.Now)));
            _cardTariffsMock = _fixture.Freeze<Mock<ICardTariffsRepository>>();
            _mockMapper = _fixture.Freeze<Mock<IMapper>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<CardTariffsService>>>();

            _service = new CardTariffsService(
                _cardTariffsMock.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        #region helpers

        private List<CardTariffsEntity> GetDefaultCardEntityList()
        {
            List<CardTariffsEntity> cardsEntity = new List<CardTariffsEntity>
            {
                _fixture.Build<CardTariffsEntity>()
                    .With(c=>c.Type, CardType.Debit)
                    .With(c=>c.CardName, "TestName")
                    .Create(),

                _fixture.Build<CardTariffsEntity>()
                    .With(c=>c.Type, CardType.Credit)
                    .Create(),

                _fixture.Build<CardTariffsEntity>()
                    .With(c=>c.Type, CardType.Credit)
                    .Create(),
            };
            return cardsEntity;
        }

        private List<CardTariffsDto> GetDefaultCardDtoList(List<CardTariffsEntity> cardsEntity)
        {
            List<CardTariffsDto> cardsDto = new List<CardTariffsDto>
            {
                _fixture.Build<CardTariffsDto>()
                    .With(c=>c.Id, cardsEntity[0].Id)
                    .With(c=>c.Type,  cardsEntity[0].Type)
                    .Create(),

                _fixture.Build<CardTariffsDto>()
                    .With(c=>c.Id, cardsEntity[1].Id)
                    .With(c=>c.Type,  cardsEntity[1].Type)
                    .Create(),

                _fixture.Build<CardTariffsDto>()
                    .With(c=>c.Id, cardsEntity[2].Id)
                    .With(c=>c.Type,  cardsEntity[2].Type)
                    .Create(),
            };
            return cardsDto;
        }

        #endregion


        #region Read
        [Fact]
        public async Task GetCardsPageAsync_WithNullArgument_ReturnsDtoList()
        {
            //Arrange
            var filtersDto = new CardTariffsFilter().ToGeneralFilters();
            List<CardTariffsEntity> cardsEntity = GetDefaultCardEntityList();
            List<CardTariffsDto> cardsDto = GetDefaultCardDtoList(cardsEntity);

            _cardTariffsMock
                .Setup(repo => repo.GetFilteredListAsync(1, It.IsAny<int>(), filtersDto.SearchFilter,
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters))
                .ReturnsAsync(cardsEntity);

            _cardTariffsMock
                .Setup(repo => repo.GetCountAsync(filtersDto.SearchFilter, filtersDto.Filters))
                .ReturnsAsync(cardsDto.Count);

            _mockMapper.Setup(m => m.Map<List<CardTariffsDto>>(It.IsAny<List<CardTariffsEntity>>()))
                .Returns(cardsDto);

            //Act
            var result = await _service.GetCardsPageAsync(null);

            //Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEquivalentTo(cardsDto);
            result.TotalCount.Should().Be(cardsDto.Count);
            result.PageNumber.Should().Be(1);

        }

        [Fact]
        public async Task GetCardsPageAsync_WithFiltredData_ReturnsFiltredDtoList()
        {
            CardTariffsFilter filters = new CardTariffsFilter
            {
                ChosenTypes = new List<CardType>
                {
                    CardType.Debit
                },
            };

            //Arrange
            var filtersDto = filters.ToGeneralFilters();
            List<CardTariffsEntity> cardsEntity = GetDefaultCardEntityList();
            List<CardTariffsEntity> cardsEntityFiltred = cardsEntity.Where(c => c.Type == CardType.Debit).ToList();

            List<CardTariffsDto> cardsDto = GetDefaultCardDtoList(cardsEntity);

            _cardTariffsMock
                .Setup(repo => repo.GetFilteredListAsync(1, It.IsAny<int>(), filtersDto.SearchFilter,
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters))
                .ReturnsAsync(cardsEntityFiltred);

            cardsDto = cardsDto.Where(c => c.Type == CardType.Debit).ToList();

            _cardTariffsMock
                .Setup(repo => repo.GetCountAsync(filtersDto.SearchFilter, It.IsAny<List<Expression<Func<CardTariffsEntity, bool>>>>()))
                .ReturnsAsync(cardsEntityFiltred.Count);


            _mockMapper.Setup(m => m.Map<List<CardTariffsDto>>(It.IsAny<List<CardTariffsEntity>>()))
                .Returns(cardsDto);

            //Act
            var result = await _service.GetCardsPageAsync(filters);

            //Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEquivalentTo(cardsDto);
            result.TotalCount.Should().Be(cardsDto.Count);
            result.PageNumber.Should().Be(1);

        }


        [Fact]
        public async Task GetCardAsync_WithNullArgument_ThrowsArgumentNullException()
        {
            //Arrange
            Guid? cardId = null;

            //Act
            Func<Task> result = async () => await _service.GetCardAsync(cardId);

            //Assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetCardAsync_WithWrongId_ReturnsNull()
        {
            //Arrange
            Guid? cardId = Guid.NewGuid();
            List<CardTariffsEntity> cardsEntity = GetDefaultCardEntityList();

            _cardTariffsMock.Setup(repo => repo.GetValueByIdAsync(cardId.Value))
                .ReturnsAsync((CardTariffsEntity?)null);

            _mockMapper.Setup(m => m.Map<CardTariffsDto?>((CardTariffsEntity?)null))
                .Returns((CardTariffsDto?)null);

            //Act
            var result = await _service.GetCardAsync(cardId);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCardAsync_WithRightId_ReturnsCardDto()
        {
            //Arrange
            List<CardTariffsEntity> cardsEntity = GetDefaultCardEntityList();
            List<CardTariffsDto> cardsDto = GetDefaultCardDtoList(cardsEntity);
            CardTariffsEntity cardTariffsEntity = cardsEntity[1];
            CardTariffsDto cardDto = cardsDto[1];
            Guid? cardId = cardTariffsEntity.Id;

            _cardTariffsMock.Setup(repo => repo.GetValueByIdAsync(cardId.Value))
                .ReturnsAsync(cardTariffsEntity);

            _mockMapper.Setup(m => m.Map<CardTariffsDto?>(It.IsAny<CardTariffsEntity>()))
                .Returns(cardDto);

            //Act
            var result = await _service.GetCardAsync(cardId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(cardDto);
        }
        #endregion


        #region Add

        [Fact]
        public async Task AddAsync_RepeatedName_ReturnErrorOperation() { 
            
            //Arrange
            CardTariffsDto newCard = _fixture.Build<CardTariffsDto>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "RepeatedName")
                    .Create();

            _cardTariffsMock
                .Setup(repo => repo.IsNameUniqueAsync(newCard.CardName))
                .ReturnsAsync(false);

            //Act
            var result = await _service.AddAsync(newCard);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task AddAsync_Unique_ReturnSuccessOperation() {

            //Arrange
            CardTariffsEntity newCardEntity = _fixture.Build<CardTariffsEntity>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "UniqueName")
                    .Create();

            CardTariffsDto newCardDto = _fixture.Build<CardTariffsDto>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "UniqueName")
                    .Create();

            _cardTariffsMock
                .Setup(repo => repo.IsNameUniqueAsync(newCardDto.CardName))
                .ReturnsAsync(true);

            _cardTariffsMock
                .Setup(repo => repo.AddAsync(It.IsAny<CardTariffsEntity>()))
                .Returns(Task.CompletedTask);

            _cardTariffsMock
                .Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<CardTariffsEntity>(It.IsAny<CardTariffsDto>()))
                .Returns(newCardEntity);

            //Act
            var result = await _service.AddAsync(newCardDto);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }


        #endregion


        #region Update
        [Fact]
        public async Task UpdateAsync_WrongId_ReturnErrorOperation()
        {
            //Arrange
            CardTariffsDto newCard = _fixture.Build<CardTariffsDto>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "UniqueName")
                    .Create();

            _cardTariffsMock
                .Setup(repo => repo.GetValueByIdAsync(newCard.Id.Value))
                .ReturnsAsync((CardTariffsEntity?)null);

            //Act
            var result = await _service.UpdateAcync(newCard);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_RepeatedName_ReturnErrorOperation()
        {
            //Arrange
            CardTariffsEntity newCardEntity = _fixture.Build<CardTariffsEntity>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "UniqueName")
                    .Create();

            CardTariffsDto newCard = _fixture.Build<CardTariffsDto>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "RepeatedName")
                    .Create();

            _cardTariffsMock
                .Setup(repo => repo.GetValueByIdAsync(newCard.Id.Value))
                .ReturnsAsync(newCardEntity);

            _cardTariffsMock
                .Setup(repo => repo.IsNameUniqueAsync(newCard.CardName))
                .ReturnsAsync(false);

            //Act
            var result = await _service.UpdateAcync(newCard);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithCorrectData_ReturnSuccessOperation()
        {

            //Arrange
            CardTariffsEntity newCardEntity = _fixture.Build<CardTariffsEntity>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "UniqueNewName")
                    .Create();

            CardTariffsDto newCardDto = _fixture.Build<CardTariffsDto>()
                    .With(c => c.Type, CardType.Debit)
                    .With(c => c.CardName, "UniqueNewName")
                    .Create();

            _cardTariffsMock
                .Setup(repo => repo.GetValueByIdAsync(newCardDto.Id.Value))
                .ReturnsAsync(newCardEntity);

            _cardTariffsMock
                .Setup(repo => repo.IsNameUniqueAsync(newCardDto.CardName))
                .ReturnsAsync(true);

            _cardTariffsMock
                .Setup(repo => repo.UpdateObject(It.IsAny<CardTariffsEntity>()));

            _cardTariffsMock
                .Setup(repo => repo.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<CardTariffsEntity>(It.IsAny<CardTariffsDto>()))
                .Returns(newCardEntity);

            //Act
            var result = await _service.UpdateAcync(newCardDto);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteAsync_WhenCardDoesNotExist_ReturnsErrorOperationResult()
        {
            // Arrange
            var cardId = Guid.NewGuid();

            _cardTariffsMock
                .Setup(r => r.GetValueByIdAsync(cardId))
                .ReturnsAsync((CardTariffsEntity?)null); 

            // Act
            var result = await _service.DeleteAsync(cardId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }


        [Fact]
        public async Task DeleteAsync_WhenCardExists_DeletesCardAndReturnsSuccess()
        {
            // Arrange
            var cardEntity = _fixture.Build<CardTariffsEntity>()
                .With(c => c.Type, CardType.Debit)
                .Create();

            _cardTariffsMock
                .Setup(r => r.GetValueByIdAsync(cardEntity.Id))
                .ReturnsAsync(cardEntity);

            _cardTariffsMock
                .Setup(r => r.DeleteElement(cardEntity));

            _cardTariffsMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(cardEntity.Id);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        #endregion

    }
}
