using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Credits;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts.Credits;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers.BankProducts
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditTariffsController : ControllerBase
    {
        private readonly ICreditTariffsService _creditTariffsService;

        public CreditTariffsController(ICreditTariffsService creditTariffsService)
        {
            _creditTariffsService = creditTariffsService;
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCreditTariffs([FromRoute] Guid id)
        {
            CreditTariffsDto? credit = await _creditTariffsService.GetCreditAsync(id);
            if (credit == null) return NotFound();
            return Ok(credit);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPage([FromQuery] CreditTariffsFilter filters)
        {
            return Ok(await _creditTariffsService.GetCreditTariffsPage(filters));
        }

        [HttpPost]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> AddCredit([FromBody] CreditTariffsDto creditTariffsDto)
        {
            OperationResult result = await _creditTariffsService.AddAsync(creditTariffsDto);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPut]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> UpdateCredit([FromBody] CreditTariffsDto creditTariffsDto)
        {
            OperationResult result = await _creditTariffsService.UpdateAcync(creditTariffsDto);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> DeleteCredit([FromRoute] Guid id)
        {
            OperationResult result = await _creditTariffsService.DeleteAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return NoContent();
        }
    }
}
