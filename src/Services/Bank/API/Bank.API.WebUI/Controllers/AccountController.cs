using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;

        public AccountController(UserManager<ApplicationUser> userManager, IIdentityService identityService)
        {
            _userManager = userManager;
            _identityService = identityService;
        }

        [HttpPost]
        public async Task<IActionResult> PostRegister([FromForm] RegisterDto registerDto)
        {
            IdentityResult result = await _identityService.RegisterAsync(registerDto);

            if (result.Succeeded) {
                return Ok(await _identityService.GetAccessToken(await _identityService.GetUserByEmailAsync(registerDto.Email)));
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostLogin([FromForm] LoginDto loginDto)
        {
            if(await _identityService.IsEmailUniqueAsync(loginDto.Email))
            {
                return BadRequest("Wrong email or password!");
            }
            bool result = await _userManager.CheckPasswordAsync(await _identityService.GetUserByEmailAsync(loginDto.Email), loginDto.Password);

            if(result)
            {
                return Ok(await _identityService.GetAccessToken(await _identityService.GetUserByEmailAsync(loginDto.Email)));
            }
            else
            {
                return BadRequest("Wrong email or password!");
            }

        }
    }
}
