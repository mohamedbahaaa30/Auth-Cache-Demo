using AuthDemo.Services.Interfaces;

namespace AuthDemo.Middlewares
{
    public class TokenBlackListMiddleware
    {
        private readonly RequestDelegate _next; 
        public TokenBlackListMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext ,ICacheService cacheService)
        {
            if(httpContext.User.Identity.IsAuthenticated !=true) //not my business if the user is not authenticated, let it pass and let the authentication middleware handle it
            {
                await _next(httpContext);
                return;  
            }

            var jti =httpContext.User.Claims.FirstOrDefault(u => u.Type == "jti")?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                await _next(httpContext);
                return;
            }

            var IsBlacklisted = await cacheService.GetDataAsync<string>($"auth:blacklist:{jti}");
            if (!string.IsNullOrEmpty(IsBlacklisted))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Token is blacklisted.");
                return;
            } 
            await _next(httpContext);
        }
    }
}
