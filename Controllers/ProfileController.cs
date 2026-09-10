using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Models.Requests;
using RumblingFishBackend.Services;
using System.Security.Claims;

namespace RumblingFishBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public ProfileController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [Authorize]
        [HttpGet("nickname")]
        public async Task<IActionResult> GetNickname()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (firebaseUid == null)
            {
                return Unauthorized();
            }

            var nickname = await _playerService.GetNicknameAsync(firebaseUid);

            return Ok(new 
            { 
                nickname = nickname 
            });
        }

        [Authorize]
        [HttpPost("nickname")]
        public async Task<IActionResult> SetNickname([FromBody] SetNicknameRequest request)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (firebaseUid == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.Nickname) || request.Nickname.Length < 3 || request.Nickname.Length > 15)
            {
                return BadRequest();
            }

            var result = await _playerService.TrySetNicknameAsync(firebaseUid, request.Nickname);

            return result switch
            {
                SetNicknameResult.Success => Ok(),
                SetNicknameResult.NicknameTaken => Conflict(),
                SetNicknameResult.PlayerNotFound => NotFound(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
