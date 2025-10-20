using Bank.API.Application.DTOs.Users;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.Users
{
    public interface IVacanciesService
    {
        public Task<PageResult<VacancyDto>> GetVacanciesPageAsync(VacancyFilters? filters);
        public Task<OperationResult> CreateVacancy(VacancyDto vacancyDto);
        public Task<OperationResult> UpdateVacancy(VacancyDto vacancyDto);
        public Task<OperationResult> DeleteVacancy(Guid vacancyId);
        public Task<VacancyDto?> GetVacancyAsync(Guid vacancyId);
    }
}
