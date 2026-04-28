namespace AuthDemo.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsExpired  => DateTime.UtcNow >= Expiration;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
