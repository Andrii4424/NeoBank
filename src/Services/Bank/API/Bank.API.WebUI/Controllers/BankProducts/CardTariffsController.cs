using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers.BankProducts
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CardTariffsController : ControllerBase
    {
        private readonly ICardTariffsService _cardTariffsService;

        public CardTariffsController(ICardTariffsService cardTariffsService)
        {
            _cardTariffsService = cardTariffsService;
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCardTariffs([FromRoute] Guid id)
        {
            CardTariffsDto? card = await _cardTariffsService.GetCardAsync(id);
            if(card == null) return NotFound();
            return Ok(card);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPage([FromQuery] CardTariffsFilter? filters)
        {
            return Ok(await _cardTariffsService.GetCardsPageAsync(filters));
        }

        [HttpPost]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> AddCard([FromBody] CardTariffsDto cardDto)
        {
            OperationResult result = await _cardTariffsService.AddAsync(cardDto);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPut]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> UpdateCard([FromBody] CardTariffsDto cardDto)
        {
            OperationResult result = await _cardTariffsService.UpdateAcync(cardDto);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> DeleteCard([FromRoute] Guid id)
        {
            OperationResult result = await _cardTariffsService.DeleteAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return NoContent();
        }
    }
}
