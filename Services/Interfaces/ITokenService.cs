using AuthDemo.Models;

namespace AuthDemo.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateAccessToken(AppUser user);
        public RefreshToken GenerateRefreshToken();
    }
}
