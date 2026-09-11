using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Models.Requests;
using RumblingFishBackend.Services;
using System.Security.Claims;

namespace RumblingFishBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public LevelController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [Authorize]
        [HttpPost("{levelId}/result")]
        public async Task<IActionResult> Result([FromRoute] int levelId, [FromBody] LevelResultRequest request)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (firebaseUid == null)
            {
                return Unauthorized();
            }

            var result = await _playerService.TryAddPlayerStatistic(firebaseUid, levelId, request);

            return result switch
            {
                SetPlayerStatisticResult.Success => Ok(),
                SetPlayerStatisticResult.Error => Problem(),
                SetPlayerStatisticResult.InvalidLevel => NotFound(),
                SetPlayerStatisticResult.PlayerNotFound => NotFound(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
