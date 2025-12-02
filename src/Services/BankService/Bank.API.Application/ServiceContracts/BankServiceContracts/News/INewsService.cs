using Bank.API.Application.DTOs.News;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.News;
using Bank.API.Domain.Entities.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.News
{
    public interface INewsService
    {
        public Task<PageResult<NewsDto>> GetNewsPageAsync(NewsFilter? filters);
        public Task<OperationResult> CreateNewsAsync(AddNewsDto news);
        public Task<OperationResult> UpdateNewsAsync(UpdateNewsDto news);
        public Task<OperationResult> DeleteNewsAsync(Guid id);
    }
}
