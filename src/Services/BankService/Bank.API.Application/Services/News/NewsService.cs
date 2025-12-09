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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bank.API.Application.Services.News
{
    public class NewsService: INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsService> _logger;
        private readonly IWebHostEnvironment _env;
        private string[] allowedAvatarExtension = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };

        public NewsService(INewsRepository newsRepository, IMapper mapper, ILogger<NewsService> logger, IWebHostEnvironment env)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
            _logger = logger;
            _env = env;
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
        public async Task<OperationResult> CreateNewsAsync(AddNewsDto news)
        {
            if(await _newsRepository.IsDuplicateNewsAsync(news.Title))
            {
                _logger.LogError("Duplicate news with Title: {Title} found", news.Title);
                return OperationResult.Error("Duplicate news");
            }

            NewsEntity newsEntity = _mapper.Map<NewsEntity>(news);
            var ext = Path.GetExtension(news.Image.FileName).ToLowerInvariant();
            if (!allowedAvatarExtension.Contains(ext))
            {
                return OperationResult.Error("Invalid image format. Allowed: JPG/JPEG, PNG, WEBP");
            }
            string fileName = $"{newsEntity.Id}{news.Image.FileName.ToLower()}";
            string absolutePath = Path.Combine(_env.WebRootPath, "uploads", "news-photos", fileName);
            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await news.Image.CopyToAsync(stream);
            }
            newsEntity.ImagePath = $"uploads/news-photos/{fileName}";

            await _newsRepository.AddAsync(newsEntity);
            await _newsRepository.SaveAsync();

            _logger.LogInformation("News created with ID: {NewsId}", newsEntity.Id);
            return OperationResult.Ok();
        }


        //Update
        public async Task<OperationResult> UpdateNewsAsync(UpdateNewsDto news)
        {
            _logger.LogInformation("Trying to update news with ID: {NewsId}", news.Id);
            NewsEntity? newsEntity = await _newsRepository.GetValueByIdAsync(news.Id);
            if (newsEntity == null)
            {
                _logger.LogError("News with ID: {NewsId} not found for deletion", news.Id);
                return OperationResult.Error("News not found");
            }

            if (news.Image != null)
            {
                //Delete old photo if exists
                if (news.ImagePath != null)
                {
                    string deletePath = Path.Combine(_env.WebRootPath, news.ImagePath);
                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }

                //Add new photo
                var ext = Path.GetExtension(news.Image.FileName).ToLowerInvariant();
                if (!allowedAvatarExtension.Contains(ext))
                {
                    return OperationResult.Error("Invalid image format. Allowed: JPG/JPEG, PNG, WEBP");
                }
                string fileName = $"{news.Id}{news.Image.FileName.ToLower()}";
                string absolutePath = Path.Combine(_env.WebRootPath, "uploads", "news-photos", fileName);
                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await news.Image.CopyToAsync(stream);
                }
                newsEntity.ImagePath = $"uploads/news-photos/{fileName}";
            }


            _newsRepository.UpdateObject(newsEntity);
            await _newsRepository.SaveAsync();

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
            if (newsEntity.ImagePath != null)
            {
                string deletePath = Path.Combine(_env.WebRootPath, newsEntity.ImagePath);
                if (File.Exists(deletePath))
                {
                    File.Delete(deletePath);
                }
            }

            _newsRepository.DeleteElement(newsEntity);
            await _newsRepository.SaveAsync();

            _logger.LogInformation("News deleted with ID: {NewsId}", newsEntity.Id);
            return OperationResult.Ok();
        }
    }
}
