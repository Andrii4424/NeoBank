using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices.BankProducts
{
    public class CardTariffsService: ICardTariffsService
    {
        private readonly ICardTariffsRepository _cardTariffsRepository;
        private readonly IMapper _mapper;
        private readonly int _pageCount = SharedMethods.GetDefaultPageCount();

        public CardTariffsService(ICardTariffsRepository cardTariffsRepository, IMapper mapper) { 
            _cardTariffsRepository = cardTariffsRepository;
            _mapper = mapper;
        }

        //Read Operations
        public async Task<PageResult<CardTariffsDto>> GetDefaultPageAsync()
        {
            List<CardTariffsEntity> cards = await _cardTariffsRepository.GetFilteredListAsync(1, _pageCount, null, true, null, null);

            PageResult<CardTariffsDto> cardsResult = new PageResult<CardTariffsDto>(_mapper.Map<List<CardTariffsDto>>(cards),
                await _cardTariffsRepository.GetCountAsync(null, null), 1, _pageCount);

            return cardsResult;
        }

        public async Task<PageResult<CardTariffsDto>> GetCardsAsync(CardTariffsFilter filters)
        {
            FiltersDto<CardTariffsEntity> filtersDto = filters.ToGeneralFilters();

            List<CardTariffsEntity> cards =  await _cardTariffsRepository.GetFilteredListAsync(filtersDto.PageNumber, filtersDto.PageSize, filtersDto.SearchFilter, 
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters);

            PageResult<CardTariffsDto> cardsResult = new PageResult<CardTariffsDto>(_mapper.Map<List<CardTariffsDto>>(cards), 
                await _cardTariffsRepository.GetCountAsync(filtersDto.SearchFilter, filtersDto.Filters), filters.PageNumber, _pageCount);

            return cardsResult;
        }

        //Create operations
        public async Task<OperationResult> AddAsync(CardTariffsDto cardDto)
        {
            if (!await _cardTariffsRepository.IsNameUniqueAsync(cardDto.CardName))
            {
                return OperationResult.Error("Card with same name is already exists. Please provide unique name");
            }
            await _cardTariffsRepository.AddAsync(_mapper.Map<CardTariffsEntity>(cardDto));

            return OperationResult.Ok();
        }

        //Update operations
        public async Task<OperationResult> UpdateAcync(CardTariffsDto cardDto)
        {
            if (!await _cardTariffsRepository.IsNameUniqueAsync(cardDto.CardName))
            {
                return OperationResult.Error("Card with same name is already exists. Please provide unique name");
            }
            CardTariffsEntity? card = await _cardTariffsRepository.GetValueByIdAsync(cardDto.Id);
            if (card==null)
            {
                return OperationResult.Error("Card with this id doesnt exist");
            }
            _cardTariffsRepository.UpdateObject(_mapper.Map(cardDto, card));

            return OperationResult.Ok();
        }

        //Delete operations
        public async Task<OperationResult> DeleteAsync(Guid cardId)
        {
            CardTariffsEntity? card = await _cardTariffsRepository.GetValueByIdAsync(cardId);

            if (card == null)
            {
                return OperationResult.Error("Card with this id doesnt exist");
            }
            _cardTariffsRepository.DeleteElement(card);
            return OperationResult.Ok();

        }
    }
}
