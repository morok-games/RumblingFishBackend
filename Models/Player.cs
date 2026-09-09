namespace RumblingFishBackend.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string? Nickname { get; set; }
        public string? CountryCode { get; set; }
    }
}
