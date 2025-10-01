using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Bank.API.Application.Services.BankServices.BankProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers.BankProducts
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyRates()
        {
            return Ok(await _currencyService.GetCurrencyData());
        }



    }
}
