using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Credits;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts.Credits;
using Bank.API.Application.Services.BankServices;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.CreditsServices
{
    public class CreditTariffsService : ICreditTariffsService
    {
        private readonly ICreditTariffsRepository _creditTariffsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreditTariffsService> _logger;

        public CreditTariffsService(ICreditTariffsRepository creditTariffsRepository, IMapper mapper, ILogger<CreditTariffsService> logger)
        {
            _creditTariffsRepository = creditTariffsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        //Read Services
        public async Task<PageResult<CreditTariffsDto>> GetCreditTariffsPage(CreditTariffsFilter? filtersDto)
        {
            _logger.LogInformation("Trying to get credit tariffs page");
            if (filtersDto == null)
            {
                filtersDto = new CreditTariffsFilter();
            }

            FiltersDto<CreditTariffsEntity> filters = filtersDto.ToGeneralFilters();
            List<CreditTariffsEntity> credits = await _creditTariffsRepository.GetFilteredListAsync(filters.PageNumber.Value, filters.PageSize.Value, filters.SearchFilter,
                filters.Ascending, filters.SortValue, filters.Filters);


            PageResult<CreditTariffsDto> pageResult = new PageResult<CreditTariffsDto>(_mapper.Map<List<CreditTariffsDto>>(credits),
                await _creditTariffsRepository.GetCountAsync(filters.SearchFilter, filters.Filters), filters.PageNumber.Value, filtersDto.PageSize);

            _logger.LogInformation("Success getting credit tariffs page");
            return pageResult;
        }

        public async Task<CreditTariffsDto?> GetCreditAsync(Guid? cardId)
        {
            _logger.LogInformation("Getting credit tarriffs info");
            if (cardId == null)
            {
                _logger.LogError("Failed getting credit tarriffs info.Card id is null");
                throw new ArgumentNullException("Credit tariffs id cant be null");
            }

            return _mapper.Map<CreditTariffsDto>(await _creditTariffsRepository.GetValueByIdAsync(cardId.Value));
        }

        //Create operations
        public async Task<OperationResult> AddAsync(CreditTariffsDto creditTariffsDto)
        {
            _logger.LogInformation("Trying to add credit tariffs");
            if (!await _creditTariffsRepository.IsNameUniqueAsync(creditTariffsDto.Name))
            {
                _logger.LogError("Error while adding credit tariffs. Card with name {creditName} is already exists.", creditTariffsDto.Name);
                return OperationResult.Error("Credit with same name is already exists. Please provide unique name");
            }

            await _creditTariffsRepository.AddAsync(_mapper.Map<CreditTariffsEntity>(creditTariffsDto));
            await _creditTariffsRepository.SaveAsync();

            _logger.LogInformation("Credit Tariffs with name {creditName} has been successfully added", creditTariffsDto.Name);
            return OperationResult.Ok();
        }

        //Update operations
        public async Task<OperationResult> UpdateAcync(CreditTariffsDto creditTariffsDto)
        {
            _logger.LogInformation("Trying to uptate credit tarriffs with id {cardId}", creditTariffsDto.Id);

            CreditTariffsEntity? creditTariffs = await _creditTariffsRepository.GetValueByIdAsync(creditTariffsDto.Id.Value);

            if (creditTariffs == null)
            {
                _logger.LogError("Error while updating credit tariffs. Credit with id {creditId} doesnt exist.", creditTariffsDto.Id);
                return OperationResult.Error("Credit with this id doesnt exist");
            }
            if (creditTariffs.Name != creditTariffsDto.Name && !await _creditTariffsRepository.IsNameUniqueAsync(creditTariffsDto.Name))
            {
                _logger.LogError("Error while upating credit tariffs. Card with name {creditName} is already exists.", creditTariffsDto.Name);
                return OperationResult.Error("Credit with same name is already exists. Please provide unique name");
            }

            _creditTariffsRepository.UpdateObject(_mapper.Map(creditTariffsDto, creditTariffs));
            await _creditTariffsRepository.SaveAsync();

            _logger.LogInformation("Credit Tariffs with id {creditId} has been successfully updated", creditTariffsDto.Id);

            return OperationResult.Ok();
        }

        //Delete operations
        public async Task<OperationResult> DeleteAsync(Guid creditId)
        {
            _logger.LogInformation("Trying to delete credit tarriffs with id {creditId}", creditId);

            CreditTariffsEntity? creditTariffs = await _creditTariffsRepository.GetValueByIdAsync(creditId);

            if (creditTariffs == null)
            {
                _logger.LogError("Error while deleting credit tariffs. Credit with id {creditId} doesnt exist", creditId);
                return OperationResult.Error("Credit with this id doesnt exist");
            }
            _creditTariffsRepository.DeleteElement(creditTariffs);
            await _creditTariffsRepository.SaveAsync();

            _logger.LogInformation("Credit Tariffs with id {creditId} has been successfully deleted", creditId);

            return OperationResult.Ok();
        }
    }
}
