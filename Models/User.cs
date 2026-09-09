namespace RumblingFishBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirebaseUid { get; set; } = null!;
        public string? Nickname { get; set; }
        public string? LastIpAddress { get; set; }
        public string? CountryCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }
    }
}
