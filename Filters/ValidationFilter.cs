using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Filter
{
    public class ValidationFilter<T> : IEndpointFilter
    {
        public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var model = context.Arguments.OfType<T>().FirstOrDefault();
            if (model == null)
                return new ValueTask<object?>(Results.BadRequest("No model provided"));

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
            {
                var Result = validationResults
                     .GroupBy(v => v.MemberNames.FirstOrDefault() ?? "Error")
                     .ToDictionary
                     (
                        g => g.Key,
                        g => g.Select(v => v.ErrorMessage).ToArray()
                     );
                return new ValueTask<object?>(Results.BadRequest(Result));
      
            }
            return next(context);
        }
    }
}
