using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers.BankProducts
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class TransactionController : ControllerBase
    {
        private readonly IUserCardService _userCardService;

        public TransactionController(IUserCardService userCardService)
        {
            _userCardService = userCardService;
        }

        [HttpPost]
        public async Task<IActionResult> GetTransactionDetails([FromBody] TransactionDetailsDto details)
        {
            try
            {
                details = await _userCardService.GetTransactionDetails(details);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(details);
        }

        [HttpGet("{cardId}/check")]
        public async Task<IActionResult> CheckBalance([FromRoute] Guid cardId, [FromQuery] decimal amount)
        {
            try
            {
                bool result = await _userCardService.IsEnoughMoney(new CardOperationDto { cardId = cardId, amount = amount });
                return Ok(result);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{cardId}")]
        public async Task<IActionResult> CheckBalance([FromRoute] Guid cardId)
        {
            return Ok(await _userCardService.GetP2PComissionByUserCardId(cardId));
        }

        [HttpPut]
        public async Task<IActionResult> ChangeBalance([FromBody] CardOperationDto operationInfo)
        {
            return Ok();
        }
    }
}
