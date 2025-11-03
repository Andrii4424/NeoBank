using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.DTOs.Users.Vacancies;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Auth;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using System.Security.Claims;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecoveryPasswordService _recoveryPasswordService;
        private readonly IIdentityService _identityService;

        public AccountController(UserManager<ApplicationUser> userManager, IIdentityService identityService, IRecoveryPasswordService recoveryPasswordService)
        {
            _userManager = userManager;
            _identityService = identityService;
            _recoveryPasswordService = recoveryPasswordService;
        }

        //Auth
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
        public async Task<IActionResult> SendRefreshCode([FromBody] ChangePasswordDto? changePasswordDetails)
        {
            OperationResult result = await _recoveryPasswordService.SetAndSendRefreshPasswordCodeAsync(changePasswordDetails.Email);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateRefreshCode([FromBody] ChangePasswordDto changePasswordDetails)
        {
            bool result = await _recoveryPasswordService.ValidateRefreshPasswordCodeAsync(changePasswordDetails.Email, changePasswordDetails.RefreshCode);
            if (!result)
            {
                return BadRequest("Code is not valid");
            }
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto changePasswordDetails)
        {
            OperationResult result = await _recoveryPasswordService.UpdatePasswordAsync(changePasswordDetails);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if(await _identityService.IsEmailUniqueAsync(loginDto.Email))
            {
                return BadRequest("Wrong email or password!");
            }
            bool passwordResult = await _userManager.CheckPasswordAsync(await _identityService.GetUserByEmailAsync(loginDto.Email), loginDto.Password);

            if(passwordResult)
            {
                AuthenticationResponse? result = await _identityService.GetAccessToken(await _identityService.GetUserByEmailAsync(loginDto.Email));

                return await TokenHelper(result);
            }
            else
            {
                return BadRequest("Wrong email or password!");
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

        [HttpGet]
        public async Task<IActionResult> GetRole()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userId == null)
            {
                return Unauthorized();
            }
            ProfileDto? user = await _identityService.GetFullProfile(userId.ToString());
            if (user == null)
            {
                return Unauthorized();
            }
            user.Role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new { role = user.Role });
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
                SameSite = SameSiteMode.None,
                Expires = result.RefreshExpiresOn
            });

            Console.WriteLine($"New Refresh Token: {result.RefreshToken}");
            return Ok(new AccessTokenDto(result.AccessToken, result.AccessExpiresOn));
        }
    }
}
