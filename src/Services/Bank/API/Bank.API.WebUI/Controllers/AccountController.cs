using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sprache;

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
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            IdentityResult result = await _identityService.RegisterAsync(registerDto);

            if (result.Succeeded) {
                AuthenticationResponse? tokenResult = await _identityService.GetAccessToken(await _identityService.GetUserByEmailAsync(registerDto.Email));

                return await TokenHelper(tokenResult);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if(await _identityService.IsEmailUniqueAsync(loginDto.Email))
            {
                return Unauthorized("Wrong email or password!");
            }
            bool passwordResult = await _userManager.CheckPasswordAsync(await _identityService.GetUserByEmailAsync(loginDto.Email), loginDto.Password);

            if(passwordResult)
            {
                AuthenticationResponse? result = await _identityService.GetAccessToken(await _identityService.GetUserByEmailAsync(loginDto.Email));

                return await TokenHelper(result);
            }
            else
            {
                return Unauthorized("Wrong email or password!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _identityService.LogoutAsync(await _userManager.GetUserAsync(User));
            Response.Cookies.Delete("refresh_token");
            return NoContent();
        }


        [HttpPost]
        public async Task<IActionResult> Refresh()
        {
            var oldRefreshToken = Request.Cookies["refresh_token"];
            if (oldRefreshToken == null) { 
                return Unauthorized();
            }
            AuthenticationResponse? result = await _identityService.CheckAndUpdateRefreshTokenAsync(oldRefreshToken);

            return await TokenHelper(result);
        }

        //Cheks Authentication Response, writes down cookie and return authorization status
        private async Task<IActionResult> TokenHelper(AuthenticationResponse? result)
        {
            if (result == null)
            {
                return Unauthorized();
            }
            //Save refresh token to Http-Only cookie
            Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = result.RefreshExpiresOn
            });
            return Ok(new AccessTokenDto(result.AccessToken, result.AccessExpiresOn));
        }

    }
}
