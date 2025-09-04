using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityService _identityService;

        public AccountController(SignInManager<ApplicationUser> signInManager, IIdentityService identityService)
        {
            _signInManager = signInManager;
            _identityService = identityService;
        }

        [HttpPost]
        public async Task<IActionResult> PostRegister([FromBody] RegisterDto registerDto)
        {
            IdentityResult result = await _identityService.RegisterAsync(registerDto);

            if (result.Succeeded) {
                await _signInManager.SignInAsync(await _identityService.GetUserByEmailAsync(registerDto.Email), isPersistent: false);
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

    }
}
