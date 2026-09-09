using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Services;
using System.Security.Claims;

namespace RumblingFishBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (firebaseUid == null)
            {
                return Unauthorized();
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var user = await _userService.GetOrCreateUserAsync(firebaseUid, ipAddress);

            return Ok(new
            {
                userId = user.Id
            });
        }
    }
}
