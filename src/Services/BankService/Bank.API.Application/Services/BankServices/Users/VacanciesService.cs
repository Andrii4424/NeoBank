using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Users;
using Bank.API.Domain.RepositoryContracts.Users;
using Microsoft.Extensions.Logging;


namespace Bank.API.Application.Services.BankServices.Users
{
    public class VacanciesService : IVacanciesService
    {
        private readonly ILogger<VacanciesService> _logger;
        private readonly IVacanciesRepository _vacanciesRepository;
        private readonly IMapper _mapper;

        public VacanciesService(ILogger<VacanciesService> logger, IVacanciesRepository vacanciesRepository, IMapper mapper)
        {
            _logger = logger;
            _vacanciesRepository = vacanciesRepository;
            _mapper = mapper;
        }

        //Read Operations
        public async Task<PageResult<VacancyDto>> GetVacanciesPageAsync(VacancyFilters? filters)
        {
            _logger.LogInformation("Trying to get vacancies page");
            filters.PageNumber = filters.PageNumber ?? 1;
            FiltersDto<VacancyEntity> filtersDto = filters.ToGeneralFilters();

            List<VacancyEntity> vacancies = await _vacanciesRepository.GetFilteredListAsync(filtersDto.PageNumber.Value, filters.PageSize.Value, filtersDto.SearchFilter,
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters);

            PageResult<VacancyDto> pageResult = new PageResult<VacancyDto>(_mapper.Map<List<VacancyDto>>(vacancies),
                await _vacanciesRepository.GetCountAsync(filtersDto.SearchFilter, filtersDto.Filters), filters.PageNumber.Value, filters.PageSize.Value);

            _logger.LogInformation("Success getting vacancies page");
            return pageResult;
        }

        public async Task<VacancyDto?> GetVacancyAsync(Guid vacancyId)
        {
            _logger.LogInformation("Trying to get vacancy with id {vacancyId}", vacancyId);

            VacancyEntity? vacancy = await _vacanciesRepository.GetValueByIdAsync(vacancyId);
            if (vacancy == null)
            {
                _logger.LogInformation("Failed getting vacancy with id {vacancyId}", vacancyId);
                return null;
            }

            _logger.LogInformation("Success getting vacancy with id {vacancyId}", vacancyId);

            return _mapper.Map<VacancyDto>(vacancy); 
        }

        //Create 
        public async Task<OperationResult> CreateVacancy(VacancyDto vacancyDto)
        {
            _logger.LogInformation("Trying to add vacancy");
            vacancyDto.BankId = SharedMethods.GetBankGuid();

            VacancyEntity vacancy = _mapper.Map<VacancyEntity>(vacancyDto);

            vacancy.PublicationDate = DateOnly.FromDateTime(DateTime.UtcNow);
            await _vacanciesRepository.AddAsync(vacancy);
            await _vacanciesRepository.SaveAsync();

            _logger.LogInformation("Vacancy {id} has been added", vacancy.Id);

            return OperationResult.Ok();
        }

        //Update
        public async Task<OperationResult> UpdateVacancy(VacancyDto vacancyDto)
        {
            _logger.LogInformation("Trying to update vacancy");
            if (vacancyDto.Id ==null)
            {
                _logger.LogError("Update vacancy failed. Vacancy id wasnt provided");
                return OperationResult.Error("Vacancy id wasnt provided");
            }
            VacancyEntity? vacancy = await _vacanciesRepository.GetValueByIdAsync(vacancyDto.Id.Value);
            if(vacancy == null)
            {
                _logger.LogError("Update vacancy failed. Vacancy not fount");
                return OperationResult.Error("Vacancy not fount");
            }
            vacancyDto.BankId = SharedMethods.GetBankGuid();
            vacancy = _mapper.Map(vacancyDto, vacancy);
            vacancy.PublicationDate = DateOnly.FromDateTime(DateTime.UtcNow);
            _vacanciesRepository.UpdateObject(vacancy);
            await _vacanciesRepository.SaveAsync();

            _logger.LogInformation("Vacancy {id} has been updated", vacancy.Id);

            return OperationResult.Ok();
        }

        //Delete 
        public async Task<OperationResult> DeleteVacancy(Guid vacancyId)
        {
            _logger.LogInformation("Delete vacancy {id}", vacancyId);

            VacancyEntity? vacancy = await _vacanciesRepository.GetValueByIdAsync(vacancyId);

            if (vacancy == null)
            {
                _logger.LogError("Delete vacancy {id} failed. Vacancy not fount", vacancyId);
                return OperationResult.Error("Vacancy not fount");
            }

            _vacanciesRepository.DeleteElement(vacancy);
            await _vacanciesRepository.SaveAsync();

            _logger.LogInformation("Vacancy {id} has been deleted", vacancy.Id);

            return OperationResult.Ok();
        }
    }
}
