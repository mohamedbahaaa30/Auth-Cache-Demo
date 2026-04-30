using AuthDemo.Dtos;
using AuthDemo.Services;
using AuthDemo.Services.Interfaces;
using AuthDemo.Sevices.Interfaces;
using Azure;

namespace AuthDemo.EndPoints
{
    public static class AuthEndPoints
    {
        public static void MapAuthEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/auth")
                           .WithTags("Authentication");

            group.MapPost("/register", Register).AddValidation<RegisterDto>();
            group.MapPost("/login", Login).AddValidation<LoginDto>();
            group.MapPost("/logout", Logout).RequireAuthorization();
            group.MapPost("/refresh-token", RefreshToken);
        }
        public static async Task<IResult> Register(RegisterDto registerDto,IAuthService _authService)
        {
            var res = await _authService.RegisterAsync(registerDto.Email, registerDto.Password);
            if (!res.Success)
                return Results.BadRequest(res.Message);

            return Results.Ok(res.Message);
        }
        public static async Task<IResult> Login(LoginDto loginDto, IAuthService _authService, HttpContext httpContext)
        {
            var res = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
            if (!res.IsSuccess)
                return Results.BadRequest("Invalid email or password");

            httpContext.Response.Cookies.Append("refreshToken", res.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Results.Ok(new
            {
                message = "user logged in successfully",
                accessToken = res.AccessToken,
                refreshToken = res.RefreshToken,
                accessTokenExpiration = res.AccessTokenExpiration
            });
        }
        public static async Task<IResult> Logout(IAuthService _authService, HttpContext httpContext,ICacheService _cacheService)
        {  
            var refreshToken = httpContext.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
                await _authService.RevokeTokenAsync(refreshToken);
            httpContext.Response.Cookies.Delete("refreshToken");
            // Blacklist the access token
            var IsSuccess = await _authService.BlacklistAccessTokenAsync(httpContext);
            return IsSuccess ? Results.Ok("user logged out successfully") : Results.Ok("user logged out successfully - with minor warning");
            return Results.Ok("user logged out successfully");
        }
        public static async Task<IResult> RefreshToken(IAuthService _authService, HttpContext httpContext)
        {
            var refreshToken = httpContext.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Results.Unauthorized();

            var newToken = await _authService.RefreshTokenAsync(refreshToken);
            if (!newToken.IsSuccess)
                return Results.Unauthorized();

            httpContext.Response.Cookies.Append("refreshToken",newToken.ResreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            } );
            return Results.Ok("token refreshed successfully");
        }
    }
}
