using AuthDemo.Dtos;

namespace AuthDemo.Sevices.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResultDto> RegisterAsync(string email, string password);
        Task<LoginResponseDto> LoginAsync(string email, string password);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
    }
}
