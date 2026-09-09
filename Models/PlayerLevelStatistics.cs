namespace RumblingFishBackend.Models
{
    public class PlayerLevelStatistics
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; } = null!;

        public int LevelId { get; set; }

        public int Deaths { get; set; }

        public int Attempts { get; set; }

        public int PlayTime { get; set; }

        public bool Completed { get; set; }
    }
}
