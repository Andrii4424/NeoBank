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
        private readonly int _pageSize = SharedMethods.GetDefaultPageSize();

        public CardTariffsService(ICardTariffsRepository cardTariffsRepository, IMapper mapper) { 
            _cardTariffsRepository = cardTariffsRepository;
            _mapper = mapper;
        }

        //Read Operations
        public async Task<PageResult<CardTariffsDto>> GetDefaultPageAsync()
        {
            int elementsCount= await _cardTariffsRepository.GetCountAsync(null, null);
            List<CardTariffsEntity> cards = await _cardTariffsRepository.GetFilteredListAsync(1, _pageSize, null, true, null, null);

            PageResult<CardTariffsDto> cardsResult = new PageResult<CardTariffsDto>(_mapper.Map<List<CardTariffsDto>>(cards),
                await _cardTariffsRepository.GetCountAsync(null, null), 1, _pageSize);

            return cardsResult;
        }

        public async Task<PageResult<CardTariffsDto>> GetCardsPageAsync(CardTariffsFilter filters)
        {
            FiltersDto<CardTariffsEntity> filtersDto = filters.ToGeneralFilters();

            List<CardTariffsEntity> cards =  await _cardTariffsRepository.GetFilteredListAsync(filtersDto.PageNumber, filtersDto.PageSize, filtersDto.SearchFilter, 
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters);

            PageResult<CardTariffsDto> cardsResult = new PageResult<CardTariffsDto>(_mapper.Map<List<CardTariffsDto>>(cards), 
                await _cardTariffsRepository.GetCountAsync(filtersDto.SearchFilter, filtersDto.Filters), filters.PageNumber, _pageSize);

            return cardsResult;
        }

        public async Task<CardTariffsDto?> GetCardAsync(Guid cardId)
        {
            return _mapper.Map<CardTariffsDto>(await _cardTariffsRepository.GetValueByIdAsync(cardId));
        }

        //Create operations
        public async Task<OperationResult> AddAsync(CardTariffsDto cardDto)
        {
            if (!await _cardTariffsRepository.IsNameUniqueAsync(cardDto.CardName))
            {
                return OperationResult.Error("Card with same name is already exists. Please provide unique name");
            }
            cardDto.BankId = SharedMethods.GetBankGuid();

            await _cardTariffsRepository.AddAsync(_mapper.Map<CardTariffsEntity>(cardDto));
            await _cardTariffsRepository.SaveAsync();

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
            cardDto.BankId= SharedMethods.GetBankGuid();
            _cardTariffsRepository.UpdateObject(_mapper.Map(cardDto, card));
            await _cardTariffsRepository.SaveAsync();

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
            await _cardTariffsRepository.SaveAsync();


            return OperationResult.Ok();
        }
    }
}
