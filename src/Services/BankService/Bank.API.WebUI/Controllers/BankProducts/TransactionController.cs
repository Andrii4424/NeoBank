using Bank.API.Application.DTOs.BankProducts;
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

    }
}
