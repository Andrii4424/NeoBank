using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CardTariffsService> _logger;

        public CardTariffsService(ICardTariffsRepository cardTariffsRepository, IMapper mapper, ILogger<CardTariffsService> logger) { 
            _cardTariffsRepository = cardTariffsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        //Read Operations
        public async Task<PageResult<CardTariffsDto>> GetCardsPageAsync(CardTariffsFilter? filters)
        {
            _logger.LogInformation("Trying to get cards tariffs page");
            if(filters == null)
            {
                filters = new CardTariffsFilter();
            }
            filters.PageNumber = filters.PageNumber ?? 1;
            FiltersDto<CardTariffsEntity> filtersDto = filters.ToGeneralFilters();

            List<CardTariffsEntity> cards =  await _cardTariffsRepository.GetFilteredListAsync(filtersDto.PageNumber.Value, _pageSize, filtersDto.SearchFilter, 
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters);

            PageResult<CardTariffsDto> pageResult = new PageResult<CardTariffsDto>(_mapper.Map<List<CardTariffsDto>>(cards), 
                await _cardTariffsRepository.GetCountAsync(filtersDto.SearchFilter, filtersDto.Filters), filters.PageNumber.Value, _pageSize);

            _logger.LogInformation("Success getting cards tariffs page");
            return pageResult;
        }

        public async Task<CardTariffsDto?> GetCardAsync(Guid? cardId)
        {
            _logger.LogInformation("Getting cards tarriffs info");
            if(cardId == null)
            {
                _logger.LogError("Failed getting cards tarriffs info.Card id is null");
                throw new ArgumentNullException("Card id cant be null");
            }
            
            return _mapper.Map<CardTariffsDto>(await _cardTariffsRepository.GetValueByIdAsync(cardId.Value));
        }
        
        //Create operations
        public async Task<OperationResult> AddAsync(CardTariffsDto cardDto)
        {
            _logger.LogInformation("Trying to add cards tariffs");
            if (!await _cardTariffsRepository.IsNameUniqueAsync(cardDto.CardName))
            {
                _logger.LogError("Error while adding cards tariffs. Card with name {cardName} is already exists.", cardDto.CardName);
                return OperationResult.Error("Card with same name is already exists. Please provide unique name");
            }
            cardDto.BankId = SharedMethods.GetBankGuid();

            await _cardTariffsRepository.AddAsync(_mapper.Map<CardTariffsEntity>(cardDto));
            await _cardTariffsRepository.SaveAsync();

            _logger.LogInformation("Card Tariffs with name {cardName} has been successfully added", cardDto.CardName);
            return OperationResult.Ok();
        }

        //Update operations
        public async Task<OperationResult> UpdateAcync(CardTariffsDto cardDto)
        {
            _logger.LogInformation("Trying to uptate cards tarriffs with id {cardId}", cardDto.Id);

            CardTariffsEntity? card = await _cardTariffsRepository.GetValueByIdAsync(cardDto.Id.Value);

            if (card == null)
            {
                _logger.LogError("Error while updating cards tariffs. Card with id {cardId} doesnt exist.", cardDto.Id);
                return OperationResult.Error("Card with this id doesnt exist");
            }
            if (card.CardName!=cardDto.CardName && !await _cardTariffsRepository.IsNameUniqueAsync(cardDto.CardName))
            {
                _logger.LogError("Error while upating cards tariffs. Card with name {cardName} is already exists.", cardDto.CardName);
                return OperationResult.Error("Card with same name is already exists. Please provide unique name");
            }

            cardDto.BankId= SharedMethods.GetBankGuid();
            _cardTariffsRepository.UpdateObject(_mapper.Map(cardDto, card));
            await _cardTariffsRepository.SaveAsync();

            _logger.LogInformation("Card Tariffs with id {cardId} has been successfully updated", cardDto.Id);

            return OperationResult.Ok();
        }

        //Delete operations
        public async Task<OperationResult> DeleteAsync(Guid cardId)
        {
            _logger.LogInformation("Trying to delete cards tarriffs with id {cardId}", cardId);

            CardTariffsEntity? card = await _cardTariffsRepository.GetValueByIdAsync(cardId);

            if (card == null)
            {
                _logger.LogError("Error while deleting cards tariffs. Card with id {cardId} doesnt exist", cardId);
                return OperationResult.Error("Card with this id doesnt exist");
            }
            _cardTariffsRepository.DeleteElement(card);
            await _cardTariffsRepository.SaveAsync();

            _logger.LogInformation("Card Tariffs with id {cardId} has been successfully deleted", cardId);

            return OperationResult.Ok();
        }
    }
}
