namespace RumblingFishBackend.Models
{
    public class PlayerLevelStatistics
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int LevelId { get; set; }

        public int Deaths { get; set; }

        public int Attempts { get; set; }

        public int PlayTime { get; set; }

        public bool Completed { get; set; }
    }
}
