namespace RumblingFishBackend.Models
{
    public class PlayerStatistics
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; } = null!;

        public int CoinsCollected { get; set; }

        public int Experience { get; set; }

        public int LevelsScore { get; set; }
        public int Score { get; set; }

    }
}
