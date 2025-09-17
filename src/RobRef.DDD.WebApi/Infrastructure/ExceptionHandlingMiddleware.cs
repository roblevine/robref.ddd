using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace RobRef.DDD.WebApi.Infrastructure;

public sealed class ExceptionHandlingMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IProblemDetailsService problemDetailsService)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = Activity.Current?.Id ?? context.TraceIdentifier;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        var (statusCode, title, detail, type, errors, logLevel) = MapException(exception);

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            _logger.Log(logLevel, exception, "Unhandled exception captured by middleware.");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = context.Request.Path
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        problemDetails.Extensions["traceId"] = correlationId;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemContext = new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = problemDetails
        };

        await _problemDetailsService.WriteAsync(problemContext);
    }

    private static (int StatusCode, string Title, string Detail, string Type, IReadOnlyDictionary<string, string[]>? Errors, LogLevel LogLevel) MapException(Exception exception) =>
        exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                validationException.Message,
                "https://robref.ddd/problems/validation-error",
                BuildErrors(validationException),
                LogLevel.Warning),
            ArgumentException argumentException => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                argumentException.Message,
                "https://robref.ddd/problems/validation-error",
                BuildErrors(argumentException),
                LogLevel.Warning),
            InvalidOperationException invalidOperationException when
                invalidOperationException.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) => (
                StatusCodes.Status409Conflict,
                "User Already Exists",
                invalidOperationException.Message,
                "https://robref.ddd/problems/user-already-exists",
                null,
                LogLevel.Warning),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected Error",
                "An unexpected error occurred. Please try again.",
                "https://robref.ddd/problems/internal-server-error",
                null,
                LogLevel.Error)
        };

    private static IReadOnlyDictionary<string, string[]>? BuildErrors(ArgumentException argumentException)
    {
        if (string.IsNullOrWhiteSpace(argumentException.ParamName))
        {
            return new Dictionary<string, string[]>
            {
                ["request"] = new[] { argumentException.Message }
            };
        }

        return new Dictionary<string, string[]>
        {
            [argumentException.ParamName] = new[] { argumentException.Message }
        };
    }

    private static IReadOnlyDictionary<string, string[]>? BuildErrors(ValidationException validationException)
    {
        var memberNames = validationException.ValidationResult?.MemberNames ?? Array.Empty<string>();
        var targetKey = memberNames.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(targetKey))
        {
            targetKey = "request";
        }

        return new Dictionary<string, string[]>
        {
            [targetKey] = new[] { validationException.ValidationResult?.ErrorMessage ?? validationException.Message }
        };
    }
}
