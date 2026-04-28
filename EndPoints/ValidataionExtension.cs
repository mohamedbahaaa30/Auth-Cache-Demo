using AuthDemo.Filter;

namespace AuthDemo.EndPoints
{
    public static class ValidataionExtension
    {
        public static RouteHandlerBuilder AddValidation<T>(this RouteHandlerBuilder builder)
        {
           return builder.AddEndpointFilter<ValidationFilter<T>>();
        }
    }
}
