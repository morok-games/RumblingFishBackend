using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RumblingFishBackend.Services;
using System.Security.Claims;

namespace RumblingFishBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetLeaderboard() 
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (firebaseUid == null)
            {
                return Unauthorized();
            }

            var player = await _playerService.GetPlayerAsync(firebaseUid);

            if (player == null) 
            {
                return NotFound();
            }

            var leaderboard = await _leaderboardService.GetLeaderboardByPlayerAsync(player, amountOfFirstPlayers, amountBeforePlayer, amountAfterPlayer);

            return Ok(leaderboard); 
        }
    }
}
