using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.API.WebUI.Controllers.BankProducts
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserCardsController : ControllerBase
    {
        private readonly IUserCardService _userCardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCardsController(IUserCardService userCardService, UserManager<ApplicationUser> userManager)
        {
            _userCardService = userCardService;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetMyCards([FromQuery] UserCardsFilter? filters)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userId == null)
            {
                return Unauthorized();
            }
            PageResult<UserCardsDto>? cards = await _userCardService.GetUserCardsAsync(Guid.Parse(userId), filters);

            return Ok(cards);
        }

        [HttpGet("{cardId}")]
        public async Task<IActionResult> GetMyCard(Guid cardId)
        {
            UserCardsDto? card = await _userCardService.GetCardByIdAsync(cardId);
            if(card == null)
            {
                return NotFound("Card not found");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userId == null)
            {
                return Unauthorized();
            }
            if(card.UserId != Guid.Parse(userId))
            {
                return Forbid();
            }
            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CreateUserCardDto cardParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userId == null)
            {
                return Unauthorized();
            }
            cardParams.UserId = Guid.Parse(userId);

            var result = await _userCardService.CreateCardAsync(cardParams);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }
    }
}
