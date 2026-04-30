using AuthDemo.Dtos;
using AuthDemo.Models;
using AuthDemo.Services;
using AuthDemo.Services.Interfaces;
using AuthDemo.Sevices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthDemo.Sevices
{
    public class AuthService : IAuthService
    {
        private readonly Data.AppContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ICacheService _cacheService;
        public AuthService(Data.AppContext context , SignInManager<AppUser> signInManager , ITokenService tokenService, ICacheService cacheService)          
        {
            _context = context;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _cacheService = cacheService;
         }
        public async Task<LoginResponseDto> LoginAsync(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(U => U.Email == email);
            if (user == null || !await _signInManager.UserManager.CheckPasswordAsync(user, password))
                return LoginResponseDto.LoginFailure();

            //generate access token and refresh token 
            var accesstoken = _tokenService.GenerateAccessToken(user);
            var refreshtoken = _tokenService.GenerateRefreshToken();

            refreshtoken.UserId = user.Id;
            user.RefreshTokens.Add(refreshtoken);

            await _context.RefreshTokens.AddAsync(refreshtoken);
            await _context.SaveChangesAsync();

            return LoginResponseDto.LoginSuccess(accesstoken, refreshtoken.Token, DateTime.UtcNow.AddMinutes(15));
           
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = _context.Users
                                .Include(u => u.RefreshTokens)
                                .FirstOrDefault(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
            if (user == null)
                return RefreshTokenResponseDto.Failure(); 
            
            var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (token == null || token.IsRevoked || token.IsExpired)
                return RefreshTokenResponseDto.Failure();
            //rotate , revoke the old token and generate new access token and refresh token
            token.IsRevoked = true;
            
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            newRefreshToken.UserId = user.Id;
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            return  new RefreshTokenResponseDto {
                AccessTokne = newAccessToken,
                ResreshToken = newRefreshToken.Token,
                IsSuccess = true,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(30)
            };
        }

        public async Task<RegisterResultDto> RegisterAsync(string email, string password)
        {
            var NewUser = await _context.Users.AnyAsync(U => U.Email == email);
            if (NewUser)
                return RegisterResultDto.FailureResult("Email already exists");

            var user = new AppUser
            {
                Email = email,
                UserName = email
            };
            await _signInManager.UserManager.CreateAsync(user, password);
            return RegisterResultDto.SuccessResult("User created successfully", user.Id);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                                .Include(u => u.RefreshTokens)
                                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
            if (user == null)
                 throw new UnauthorizedAccessException("Invalid token");

            var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (token == null || token.IsRevoked || token.IsExpired)
                throw new UnauthorizedAccessException("Invalid token");

            token.IsRevoked = true;
        }
                
        public async Task<bool> BlacklistAccessTokenAsync(HttpContext httpContext)
        {
            //blacklist the access token 
            var jti = httpContext.User.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
            var exp = httpContext.User.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(exp))
                return false;
            //cast exp to long and calculate the expiration time
            var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));

            if (!string.IsNullOrEmpty(jti))
                await _cacheService.SetDataAsync($"auth:blacklist:{jti}", jti, expTime);
            return true;
        }
    }
}
