using AutoMapper;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.News;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.Helpers.HelperClasses.Filters.News;
using Bank.API.Application.ServiceContracts.BankServiceContracts.News;
using Bank.API.Application.Services.BankServices.BankProducts;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.News;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Domain.RepositoryContracts.News;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.News
{
    public class NewsService: INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsService> _logger;

        public NewsService(INewsRepository newsRepository, IMapper mapper, ILogger<NewsService> logger)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        //Read
        public async Task<PageResult<NewsDto>> GetNewsPageAsync(NewsFilter? filters)
        {
            _logger.LogInformation("Trying to get news tariffs page");
            if (filters == null)
            {
                filters = new NewsFilter();
            }
            filters.PageNumber = filters.PageNumber ?? 1;
            FiltersDto<NewsEntity> filtersDto = filters.ToGeneralFilters();

            List<NewsEntity> cards = await _newsRepository.GetFilteredListAsync(filtersDto.PageNumber.Value, filters.PageSize.Value, filtersDto.SearchFilter,
                filtersDto.Ascending, filtersDto.SortValue, filtersDto.Filters);

            PageResult<NewsDto> pageResult = new PageResult<NewsDto>(_mapper.Map<List<NewsDto>>(cards),
                await _newsRepository.GetCountAsync(filtersDto.SearchFilter, filtersDto.Filters), filters.PageNumber.Value, filters.PageSize.Value);

            _logger.LogInformation("Success getting cards tariffs page");

            return pageResult;
        }
        

        //Create
        public async Task<OperationResult> CreateNewsAsync(NewsDto news)
        {
            if(await _newsRepository.IsDuplicateNewsAsync(news.Id.Value))
            {
                _logger.LogError("Duplicate news with ID: {NewsId} found", news.Id);
                return OperationResult.Error("Duplicate news");
            }
            NewsEntity newsEntity = _mapper.Map<NewsEntity>(news);
            await _newsRepository.AddAsync(newsEntity);
            _logger.LogInformation("News created with ID: {NewsId}", newsEntity.Id);
            return OperationResult.Ok();
        }


        //Update
        public async Task<OperationResult> UpdateNewsAsync(NewsDto news)
        {
            _logger.LogInformation("Trying to update news with ID: {NewsId}", news.Id);
            NewsEntity? newsEntity = await _newsRepository.GetValueByIdAsync(news.Id.Value);
            if (newsEntity == null)
            {
                _logger.LogError("News with ID: {NewsId} not found for deletion", news.Id);
                return OperationResult.Error("News not found");
            }

            _newsRepository.UpdateObject(newsEntity);
            _logger.LogInformation("News updated with ID: {NewsId}", newsEntity.Id);
            return OperationResult.Ok();
        }


        //Delete
        public async Task<OperationResult> DeleteNewsAsync(Guid id)
        {
            _logger.LogInformation("Trying to delete news with ID: {NewsId}", id);
            NewsEntity? newsEntity = await _newsRepository.GetValueByIdAsync(id);
            if (newsEntity == null)
            {
                _logger.LogError("News with ID: {NewsId} not found for deletion", id);
                return OperationResult.Error("News not found");
            }

            _newsRepository.DeleteElement(newsEntity);
            _logger.LogInformation("News deleted with ID: {NewsId}", newsEntity.Id);
            return OperationResult.Ok();
        }
    }
}
