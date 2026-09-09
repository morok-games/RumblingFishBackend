using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RumblingFishBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        [HttpGet] public IActionResult GetLeaderboard() 
        { 
            return Ok(
                new { 
                    message = "Leaderboard works!",
                    scores = new[] 
                    { 
                        new { player = "Player1", score = 1500 }, 
                        new { player = "Player2", score = 1200 }, 
                        new { player = "Player3", score = 800 } 
                    } 
                }); 
        }
    }
}
