using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.DTOs.Users.Vacancies;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Auth;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IVacanciesService _vacanciesService;


        public ProfileController(IIdentityService identityService, IVacanciesService vacanciesService)
        {
            _identityService = identityService;
            _vacanciesService = vacanciesService;
        }

        //Profile
        [HttpGet]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userId == null)
            {
                return NotFound();
            }
            ProfileDto? user = await _identityService.GetFullProfile(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }
            user.Role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(user);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> CroppedProfileInfo([FromRoute] Guid? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            CroppedProfileDto? user = await _identityService.GetCroppedProfile(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpGet("{userId}")]
        [Authorize(Policy ="AdminsOnly")]
        public async Task<IActionResult> FullProfileInfo([FromRoute] Guid? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            ProfileDto? user = await _identityService.GetFullProfile(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] UserFilter filters)
        {
            return Ok(await _identityService.GetUsersAsync(filters));
        }

        [HttpGet]
        public async Task<IActionResult> Employees([FromQuery] UserFilter filters)
        {
            return Ok(await _identityService.GetEmployeesAsync(filters));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOwnProfile([FromForm]ProfileDto profile)
        {
            OperationResult result = await _identityService.UpdateProfile(profile);
            if (!result.Success) {
                return BadRequest(result.ErrorMessage);
            }

            ProfileDto? updatedProfile = await _identityService.GetFullProfile(profile.Id.ToString());
            updatedProfile.Role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(updatedProfile);
        }

        [HttpPost("{vacancyId}")]
        [Authorize]
        public async Task<IActionResult> ApplyForJob([FromRoute] Guid vacancyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userId == null)
            {
                return Unauthorized();
            }
            VacancyDto? vacancy = await _vacanciesService.GetVacancyAsync(vacancyId);
            if (vacancy == null) return NotFound("Vacancy not found");
            OperationResult result = await _identityService.ApplyForJobAsync(userId, vacancy);
            if (!result.Success) return BadRequest(result.ErrorMessage);

            return Ok();
        }
    }
}
