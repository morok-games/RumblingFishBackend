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
        private readonly PlayerService _playerService;

        public AuthController(UserService userService, PlayerService playerService)
        {
            _userService = userService;
            _playerService = playerService;
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
            var player = await _playerService.GetPlayerAsync(firebaseUid);

            if (player == null)
            {
                return Problem();
            }

            return Ok(new
            {
                nickname = player.Nickname
            });
        }
    }
}
