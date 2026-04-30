using AuthDemo.Middlewares;

namespace AuthDemo.EndPoints
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseTokenBlackList (this IApplicationBuilder builder) 
            => builder.UseMiddleware<TokenBlackListMiddleware>();

    }
}
