using Bank.API.Application.DTOs.Credits;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts.Credits;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers.Users
{
    [Route("api/credits/users")]
    [ApiController]
    [Authorize]
    public class UserCreditsController : ControllerBase
    {
        private readonly IUserCreditsService _userCreditsService;

        private readonly UserManager<ApplicationUser> _userManager;

        public UserCreditsController(IUserCreditsService userCreditsService, UserManager<ApplicationUser> userManager)
        {
            _userCreditsService = userCreditsService;
            _userManager = userManager;
        }

        private async Task<Guid?> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCredits([FromQuery] UserCreditsFilter filters)
        {
            Guid? userId = await GetCurrentUserIdAsync();

            if (userId == null) {
                return Unauthorized();
            }
            return Ok(await _userCreditsService.GetUserCredits(filters, userId.Value));
        }
    }
}
