namespace RumblingFishBackend.Models.Requests
{
    public record LevelResultRequest(int Deaths, int PlayTime, int Rating, bool Completed, int Experience, int CoinsCollected);
}
