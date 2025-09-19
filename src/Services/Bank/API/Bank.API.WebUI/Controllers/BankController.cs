using Bank.API.Application.DTO;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Domain.Entities;
using Bank.API.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BankController (IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> BankInfo()
        {
            BankDto bankDto = await _bankService.GetBankInfo();
            return Ok(bankDto);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateBank([FromBody] BankDto bankDto)
        {
            OperationResult result = await _bankService.UpdateBank(bankDto);
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
