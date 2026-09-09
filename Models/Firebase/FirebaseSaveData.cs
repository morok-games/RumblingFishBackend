namespace RumblingFishBackend.Models.Firebase
{
    public class FirebaseSaveData
    {
        public FirebaseGameStatistics? gameStatistics { get; set; }
        public List<int>? completedLevels { get; set; }
    }

    public class FirebaseGameStatistics
    {
        public int Experience { get; set; }
        public int CoinsCollected { get; set; }
    }
}
