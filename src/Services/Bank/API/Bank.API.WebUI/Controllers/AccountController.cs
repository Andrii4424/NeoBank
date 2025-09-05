using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public async Task<IActionResult> PostRegister([FromForm] RegisterDto registerDto, [FromForm] bool rememberMe)
        {
            IdentityResult result = await _identityService.RegisterAsync(registerDto);

            if (result.Succeeded) {
                await _signInManager.SignInAsync(await _identityService.GetUserByEmailAsync(registerDto.Email), isPersistent: rememberMe);
                return Ok(_identityService.GetJwt(await _identityService.GetUserByEmailAsync(registerDto.Email)));
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostLogin([FromForm] LoginDto loginDto, [FromForm] bool rememberMe)
        {
            _identityService.IsEmailUniqueAsync(loginDto.Email).Wait();

            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: rememberMe, lockoutOnFailure: false);

            if(result.Succeeded)
            {
                return Ok(_identityService.GetJwt(await _identityService.GetUserByEmailAsync(loginDto.Email)));
            }
            else
            {
                return BadRequest("Wrong email or password!");
            }

        }
    }
}
