using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Filters;

/// <summary>
/// Endpoint filter that validates the request's data annotations BEFORE the handler runs —
/// the "onion" pattern from Day 1 middleware, scoped to a single endpoint. Short-circuits with
/// 400 ProblemDetails if invalid; otherwise calls next() to run the handler.
/// </summary>
public sealed class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Validate every argument that carries data annotations (e.g. the request DTO).
        foreach (object? argument in context.Arguments)
        {
            if (argument is null) continue;

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(argument, new ValidationContext(argument), validationResults, validateAllProperties: true))
            {
                Dictionary<string, string[]> errors = validationResults.ToDictionary(
                    result => result.MemberNames.FirstOrDefault() ?? string.Empty,
                    result => new[] { result.ErrorMessage ?? "Invalid value." });
                return Results.ValidationProblem(errors);          // short-circuit: 400, handler never runs
            }
        }

        return await next(context);                               // all valid -> run the handler
    }
}
