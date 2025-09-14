using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.ServiceContracts;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.API.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public ProfileController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        //Profile
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userId == null)
            {
                return NotFound();
            }
            ProfileDto? user = await _identityService.GetProfile(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }
            user.Role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(user);
        }

    }
}
