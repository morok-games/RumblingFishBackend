namespace RumblingFishBackend.Models.DTO.Leaderboard
{
    public record LeaderboardResponse(
        List<LeaderboardEntry> Top,
        List<LeaderboardEntry> Before,
        LeaderboardEntry Me,
        List<LeaderboardEntry> After);

    public record LeaderboardEntry(int Rank, string Nickname, string? CountryCode, int Score);
}
