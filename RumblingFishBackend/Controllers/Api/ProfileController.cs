using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Controllers.Api.Base;
using RumblingFishBackend.Models.DTO.Profile;
using RumblingFishBackend.Services;

namespace RumblingFishBackend.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : AuthorizedApiController
    {
        private readonly PlayerService _playerService;

        public ProfileController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("nickname")]
        public async Task<IActionResult> GetNickname()
        {
            var player = await _playerService.GetPlayerAsync(FirebaseUid);

            if (player == null || player.Nickname == null)
            {
                return NotFound();
            }

            return Ok(new NicknameResponse(player.Nickname));
        }

        [HttpPut("nickname")]
        public async Task<IActionResult> SetNickname([FromBody] SetNicknameRequest request)
        {
            var result = await _playerService.TrySetNicknameAsync(FirebaseUid, request.Nickname);

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
