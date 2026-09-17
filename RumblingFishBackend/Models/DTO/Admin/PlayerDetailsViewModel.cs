namespace RumblingFishBackend.Models.DTO.Admin
{
    public record PlayerLevel(string LevelName, int Attempts, int Deaths, int Rating, bool Completed, int PlayTime);
    public record PlayerDetailsViewModel(int PlayerId, string Nickname, string? CountryCode, string? CountryName, int Rank, int Score, DateTime CreatedAt, List<PlayerLevel> LevelStats);
}
