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

        [HttpPost]
        public async Task<IActionResult> AddNews([FromBody] NewsDto news)
        {
            OperationResult result= await _newsService.CreateNewsAsync(news);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNews([FromBody] NewsDto news)
        {
            OperationResult result = await _newsService.UpdateNewsAsync(news);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNews([FromBody] Guid id)
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
