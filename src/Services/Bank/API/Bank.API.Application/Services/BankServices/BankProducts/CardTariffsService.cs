using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices.BankProducts
{
    public class CardTariffsService
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
    }
}
