namespace RumblingFishBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirebaseUid { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime LastLoginAt { get; set; }
    }
}
