using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.DTOs.News;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.News;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Auth;
using Bank.API.Application.ServiceContracts.BankServiceContracts.News;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNews([FromQuery] NewsFilter filters)
        {
            return Ok(await _newsService.GetNewsPageAsync(filters));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById([FromRoute] Guid id)
        {
            NewsDto? news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return Ok(news);
        }

        [HttpPost]
        public async Task<IActionResult> AddNews([FromForm] AddNewsDto news)
        {
            OperationResult result= await _newsService.CreateNewsAsync(news);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNews([FromForm] UpdateNewsDto news)
        {
            OperationResult result = await _newsService.UpdateNewsAsync(news);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews([FromRoute] Guid id)
        {
            OperationResult result = await _newsService.DeleteNewsAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

    }
}
