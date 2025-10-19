using Bank.API.Application.DTO;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers.Users
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VacanciesController : ControllerBase
    {
        private readonly IVacanciesService _vacanciesService;

        public VacanciesController(IVacanciesService vacanciesService)
        {
            _vacanciesService = vacanciesService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPage([FromQuery] VacancyFilters? filters)
        {
            return Ok(await _vacanciesService.GetVacanciesPageAsync(filters));
        }

        [HttpPost]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> AddVacancy([FromBody] VacancyDto vacancy)
        {
            OperationResult result = await _vacanciesService.CreateVacancy(vacancy);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPut]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> UpdateVacancy([FromBody] VacancyDto vacancy)
        {
            OperationResult result = await _vacanciesService.UpdateVacancy(vacancy);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> DeleteVacancy([FromRoute] Guid id)
        {
            OperationResult result = await _vacanciesService.DeleteVacancy(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok();
        }
    }
}
