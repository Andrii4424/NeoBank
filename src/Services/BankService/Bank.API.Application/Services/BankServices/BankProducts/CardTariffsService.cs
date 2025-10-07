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

        public async Task<PageResult<CardTariffsDto>> GetCardsPageAsync(CardTariffsFilter? filters)
        {
            filters.PageNumber = filters.PageNumber ?? 1;
            FiltersDto<CardTariffsEntity> filtersDto = filters.ToGeneralFilters();

            List<CardTariffsEntity> cards =  await _cardTariffsRepository.GetFilteredListAsync(filtersDto.PageNumber.Value, _pageSize, filtersDto.SearchFilter, 
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters);

            PageResult<CardTariffsDto> pageResult = new PageResult<CardTariffsDto>(_mapper.Map<List<CardTariffsDto>>(cards), 
                await _cardTariffsRepository.GetCountAsync(filtersDto.SearchFilter, filtersDto.Filters), filters.PageNumber.Value, _pageSize);

            return pageResult;
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
            CardTariffsEntity? card = await _cardTariffsRepository.GetValueByIdAsync(cardDto.Id.Value);

            if (card == null)
            {
                return OperationResult.Error("Card with this id doesnt exist");
            }
            if (card.CardName!=cardDto.CardName && !await _cardTariffsRepository.IsNameUniqueAsync(cardDto.CardName))
            {
                return OperationResult.Error("Card with same name is already exists. Please provide unique name");
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
