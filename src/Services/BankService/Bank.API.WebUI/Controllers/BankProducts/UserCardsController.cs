using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.DTOs.Users.Cards;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
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
        private readonly ICurrencyService _currencyService;

        private readonly UserManager<ApplicationUser> _userManager;

        public UserCardsController(IUserCardService userCardService, UserManager<ApplicationUser> userManager,
            ICurrencyService currencyService)
        {
            _userCardService = userCardService;
            _userManager = userManager;
            _currencyService = currencyService;
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

        [HttpGet("{userId}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> GetUserCards([FromRoute] Guid userId)
        {
            return Ok(await _userCardService.GetUnfiltredUserCardsAsync(userId));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCroppedUserCards([FromRoute] Guid userId)
        {
            return Ok(await _userCardService.GetUnfiltredCroppedUserCardsAsync(userId));
        }


        [HttpGet("{cardId}")]
        public async Task<IActionResult> GetUsersCardInfo(Guid cardId)
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

        [HttpPut]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto pinParams)
        {
            OperationResult result = await _userCardService.UpdateCardPinAsync(pinParams.cardId, pinParams.newPin);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> CreditLimitExchange([FromQuery] ExchangeCurrencyDto exchangeParams)
        {
            decimal? amount;
            try
            {
                amount = await _currencyService.ExchangeCurrency(exchangeParams);
            }
            catch (Exception ex) {
                return StatusCode(500, "Unexpected error");
            }
            return Ok(amount);
        }

        [HttpPut]
        public async Task<IActionResult> ChangeCreditLimit([FromBody] ChangeCreditLimitDto limitParams)
        {
            OperationResult result = await _userCardService.UpdateCardCreditLimitAsync(limitParams.CardId, limitParams.NewCreditLimit);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPut("{cardId:guid}")]
        public async Task<IActionResult> ReissueCard([FromRoute]Guid cardId)
        {
            OperationResult result = await _userCardService.ReissueCardAcync(cardId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeCardStatus([FromBody] ChangeStatusDto newStatusParams)
        {
            OperationResult result = await _userCardService.UpdateCardStatusAsync(newStatusParams);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpDelete("{cardId}")]
        public async Task<IActionResult> CloseCard([FromRoute] Guid cardId)
        {
            OperationResult result = await _userCardService.DeleteCardAsync(cardId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }
    }
}
