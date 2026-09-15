using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Controllers.Api.Base;
using RumblingFishBackend.Models.DTO.Auth;
using RumblingFishBackend.Services;

namespace RumblingFishBackend.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : AuthorizedApiController
    {
        private readonly UserService _userService;
        private readonly PlayerService _playerService;

        public AuthController(UserService userService, PlayerService playerService)
        {
            _userService = userService;
            _playerService = playerService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var user = await _userService.GetOrCreateUserAsync(FirebaseUid, ipAddress);
            var player = await _playerService.GetPlayerAsync(FirebaseUid);

            if (player == null || player.Nickname == null)
            {
                return Problem();
            }

            return Ok(new LoginResponse(player.Nickname));
        }
    }
}
