using System.ComponentModel.DataAnnotations;

namespace RumblingFishBackend.Models.DTO.Level
{
    public record LevelResultRequest(
        [Range(0, int.MaxValue)] int Deaths,
        [Range(0, int.MaxValue)] int PlayTime,
        [Range(0, Models.Level.MaxLevelRating)] int Rating, 
        bool Completed,
        [Range(0, int.MaxValue)] int Experience,
        [Range(0, int.MaxValue)] int CoinsCollected);
}
