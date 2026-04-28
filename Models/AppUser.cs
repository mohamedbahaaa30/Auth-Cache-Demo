using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Models
{
    public class AppUser : IdentityUser
    {
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
