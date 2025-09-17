using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using RobRef.DDD.WebApi.Infrastructure;

namespace RobRef.DDD.WebApi.Tests.Infrastructure;

public sealed class ExceptionHandlingMiddlewareTests
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public ExceptionHandlingMiddlewareTests()
    {
        _logger = new TestLogger();
        _problemDetailsService = new TestProblemDetailsService();
    }

    [Fact]
    public async Task HandleException_DatabaseConnectionError_Returns500()
    {
        // Arrange
        var context = CreateHttpContext();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Connection string 'SqlServer' is not configured."),
            _logger,
            _problemDetailsService);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        var problemDetails = GetProblemDetailsFromResponse(context);
        Assert.Equal("Unexpected Error", problemDetails.Title);
        Assert.Equal("An unexpected error occurred. Please try again.", problemDetails.Detail);
    }

    [Fact]
    public async Task HandleException_UserAlreadyExists_Returns409()
    {
        // Arrange
        var context = CreateHttpContext();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("User with email 'test@example.com' already exists."),
            _logger,
            _problemDetailsService);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(409, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        var problemDetails = GetProblemDetailsFromResponse(context);
        Assert.Equal("User Already Exists", problemDetails.Title);
        Assert.Equal("User with email 'test@example.com' already exists.", problemDetails.Detail);
    }

    [Fact]
    public async Task HandleException_OtherInvalidOperationException_Returns500()
    {
        // Arrange
        var context = CreateHttpContext();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Some other operation failed."),
            _logger,
            _problemDetailsService);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        var problemDetails = GetProblemDetailsFromResponse(context);
        Assert.Equal("Unexpected Error", problemDetails.Title);
        Assert.Equal("An unexpected error occurred. Please try again.", problemDetails.Detail);
    }

    [Fact]
    public async Task HandleException_ArgumentException_Returns400()
    {
        // Arrange
        var context = CreateHttpContext();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ArgumentException("Invalid parameter value", "paramName"),
            _logger,
            _problemDetailsService);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        var problemDetails = GetProblemDetailsFromResponse(context);
        Assert.Equal("Validation Failed", problemDetails.Title);
        Assert.Equal("Invalid parameter value (Parameter 'paramName')", problemDetails.Detail);
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/test";
        return context;
    }

    private static ProblemDetailsResponse GetProblemDetailsFromResponse(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<ProblemDetailsResponse>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        })!;
    }

    private sealed class ProblemDetailsResponse
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public int Status { get; set; }
        public string? Type { get; set; }
        public string? Instance { get; set; }
    }

    private sealed class TestLogger : ILogger<ExceptionHandlingMiddleware>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    private sealed class TestProblemDetailsService : IProblemDetailsService
    {
        public bool CanWrite(ProblemDetailsContext context) => true;

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            var json = JsonSerializer.Serialize(context.ProblemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            context.HttpContext.Response.WriteAsync(json);
            return ValueTask.CompletedTask;
        }
    }
}