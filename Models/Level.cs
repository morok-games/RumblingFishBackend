namespace RumblingFishBackend.Models
{
    public class Level
    {
        public const int MaxLevelRating = 3;

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Reward { get; set; }
    }
}
