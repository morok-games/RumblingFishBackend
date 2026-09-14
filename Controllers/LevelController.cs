using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Controllers.Base;
using RumblingFishBackend.Models.DTO.Level;
using RumblingFishBackend.Services;

namespace RumblingFishBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : AuthorizedApiController
    {
        private readonly PlayerService _playerService;

        public LevelController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("{levelId}/result")]
        public async Task<IActionResult> Result([FromRoute] int levelId, [FromBody] LevelResultRequest request)
        {
            var result = await _playerService.TryAddPlayerStatistic(FirebaseUid, levelId, request);

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
