using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RobRef.DDD.WebApi.Infrastructure;

public sealed class ValidationEndpointFilter<TRequest> : IEndpointFilter where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            return await next(context);
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);

        if (!Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true))
        {
            var errors = validationResults
                .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty)
                    .Select(member => new { Member = member, Error = result.ErrorMessage ?? "Validation failed." }))
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Member) ? "request" : x.Member)
                .ToDictionary(group => group.Key, group => group.Select(x => x.Error).Distinct().ToArray(), StringComparer.OrdinalIgnoreCase);

            return Results.Problem(
                detail: "Request validation failed. See errors for details.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Failed",
                type: "https://robref.ddd/problems/validation-error",
                extensions: new Dictionary<string, object?>
                {
                    ["errors"] = errors
                });
        }

        return await next(context);
    }
}
