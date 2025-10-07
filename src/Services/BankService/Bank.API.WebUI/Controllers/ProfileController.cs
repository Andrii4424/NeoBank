using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.Helpers.HelperClasses;
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

        [HttpGet]
        public async Task<IActionResult> ProfileInfo([FromRoute] Guid? userId)
        {
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

        [HttpPost]
        public async Task<IActionResult> UpdateOwnProfile([FromForm]ProfileDto profile)
        {
            OperationResult result = await _identityService.UpdateProfile(profile);
            if (!result.Success) {
                return BadRequest(result.ErrorMessage);
            }

            ProfileDto? updatedProfile = await _identityService.GetProfile(profile.Id.ToString());
            updatedProfile.Role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(updatedProfile);
        }

    }
}
