using Bank.API.Application.DTO;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankReadService _bankReadService;
        private readonly IBankUpdateService _bankUpdateService;

        public BankController (IBankReadService bankReadService, IBankUpdateService bankUpdateService)
        {
            _bankReadService = bankReadService;
            _bankUpdateService = bankUpdateService;
        }

        [HttpGet]
        public async Task<IActionResult> BankInfo()
        {
            BankDto bankDto = await _bankReadService.GetBankInfo();
            return Ok(bankDto);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateBank([FromBody] BankDto bankDto)
        {
            OperationResult result = await _bankUpdateService.UpdateBank(bankDto);
            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
    }
}
