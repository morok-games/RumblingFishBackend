namespace RumblingFishBackend.Models
{
    public class PlayerStatistics
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int Experience { get; set; }

        public int CoinsCollected { get; set; }
    }
}
