using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Controllers.Base;
using RumblingFishBackend.Services;

namespace RumblingFishBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : AuthorizedApiController
    {
        private const int amountOfFirstPlayers = 10;
        private const int amountBeforePlayer = 3;
        private const int amountAfterPlayer = 3;
        private readonly PlayerService _playerService;
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardController(PlayerService playerService, LeaderboardService leaderboardService)
        {
            _playerService = playerService;
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaderboard() 
        {
            var player = await _playerService.GetPlayerAsync(FirebaseUid);

            if (player == null) 
            {
                return NotFound();
            }

            var leaderboard = await _leaderboardService.GetLeaderboardByPlayerAsync(player, amountOfFirstPlayers, amountBeforePlayer, amountAfterPlayer);

            return Ok(leaderboard); 
        }
    }
}
