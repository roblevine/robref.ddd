using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RobRef.DDD.Application.Users.Services;
using RobRef.DDD.Infrastructure.Configuration;
using RobRef.DDD.WebApi.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (!context.ProblemDetails.Extensions.ContainsKey("traceId"))
        {
            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        }

        if (context.ProblemDetails.Status == StatusCodes.Status500InternalServerError)
        {
            context.ProblemDetails.Type ??= "https://robref.ddd/problems/internal-server-error";
            context.ProblemDetails.Title ??= "Unexpected Error";
            context.ProblemDetails.Detail ??= "An unexpected error occurred. Please try again.";
        }
    };

    options.Map<ArgumentException>((httpContext, exception) =>
        CreateProblemDetails(
            StatusCodes.Status400BadRequest,
            "Validation Failed",
            "https://robref.ddd/problems/validation-error",
            exception.Message,
            httpContext));

    options.Map<InvalidOperationException>((httpContext, exception) =>
        CreateProblemDetails(
            StatusCodes.Status409Conflict,
            "User Already Exists",
            "https://robref.ddd/problems/user-already-exists",
            exception.Message,
            httpContext));

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RobRef.DDD User API",
        Version = "v1",
        Description = "API endpoints for user registration."
    });
});

var connectionString = builder.Configuration.GetConnectionString("SqlServer");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddInfrastructureWithEfCore(connectionString);
}
else
{
    builder.Services.AddInfrastructure();
}

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/users/register",
        async Task<IResult> (RegisterUserRequest request, UserApplicationService userService, HttpContext httpContext) =>
        {
            var validationErrors = request.Validate();
            if (validationErrors is not null && validationErrors.Count > 0)
            {
                return Results.ValidationProblem(
                    errors: validationErrors,
                    type: "https://robref.ddd/problems/validation-error",
                    title: "Validation Failed",
                    detail: "Request validation failed. See errors for details.");
            }

            var userId = await userService.RegisterUserAsync(
                request.Email,
                request.NormalizeTitle(),
                request.FirstName,
                request.LastName,
                httpContext.RequestAborted);

            var id = userId.ToString();
            var location = $"/api/users/{id}";

            return Results.Created(location, new RegisterUserResponse(id));
        })
    .WithName("RegisterUser")
    .WithTags("Users")
    .Produces<RegisterUserResponse>(StatusCodes.Status201Created, "application/json")
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Register a new user";
        operation.Description = "Creates a new user using the domain-driven registration flow.";

        if (!operation.Responses.TryGetValue(StatusCodes.Status201Created.ToString(), out var createdResponse))
        {
            createdResponse = new OpenApiResponse();
            operation.Responses[StatusCodes.Status201Created.ToString()] = createdResponse;
        }

        createdResponse.Description = "User registered successfully.";
        createdResponse.Content ??= new Dictionary<string, OpenApiMediaType>();
        createdResponse.Content["application/json"] = new OpenApiMediaType
        {
            Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("01HXZ7J6FNT3E8PJ4A8J7Z5WKR")
            }
        };

        if (!operation.Responses.TryGetValue(StatusCodes.Status400BadRequest.ToString(), out var badRequest))
        {
            badRequest = new OpenApiResponse();
            operation.Responses[StatusCodes.Status400BadRequest.ToString()] = badRequest;
        }

        badRequest.Description = "Request validation failed.";
        badRequest.Content ??= new Dictionary<string, OpenApiMediaType>();
        if (!badRequest.Content.TryGetValue("application/problem+json", out var validationMediaType))
        {
            validationMediaType = new OpenApiMediaType();
            badRequest.Content["application/problem+json"] = validationMediaType;
        }

        validationMediaType.Example = new OpenApiObject
        {
            ["type"] = new OpenApiString("https://robref.ddd/problems/validation-error"),
            ["title"] = new OpenApiString("Validation Failed"),
            ["status"] = new OpenApiInteger(StatusCodes.Status400BadRequest),
            ["detail"] = new OpenApiString("Request validation failed. See errors for details."),
            ["errors"] = new OpenApiObject
            {
                ["Email"] = new OpenApiArray
                {
                    new OpenApiString("Email must be a valid email address.")
                }
            }
        };

        if (!operation.Responses.TryGetValue(StatusCodes.Status409Conflict.ToString(), out var conflict))
        {
            conflict = new OpenApiResponse();
            operation.Responses[StatusCodes.Status409Conflict.ToString()] = conflict;
        }

        conflict.Description = "User already exists.";
        conflict.Content ??= new Dictionary<string, OpenApiMediaType>();
        if (!conflict.Content.TryGetValue("application/problem+json", out var conflictMediaType))
        {
            conflictMediaType = new OpenApiMediaType();
            conflict.Content["application/problem+json"] = conflictMediaType;
        }

        conflictMediaType.Example = new OpenApiObject
        {
            ["type"] = new OpenApiString("https://robref.ddd/problems/user-already-exists"),
            ["title"] = new OpenApiString("User Already Exists"),
            ["status"] = new OpenApiInteger(StatusCodes.Status409Conflict),
            ["detail"] = new OpenApiString("User with email 'user@example.com' already exists."),
            ["traceId"] = new OpenApiString("00-00000000000000000000000000000000-0000000000000000-00")
        };

        return operation;
    });

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health")
    .WithTags("Health")
    .Produces(StatusCodes.Status200OK);

app.Run();

static ProblemDetails CreateProblemDetails(int statusCode, string title, string type, string detail, HttpContext context)
{
    var problem = new ProblemDetails
    {
        Status = statusCode,
        Title = title,
        Type = type,
        Detail = detail
    };

    problem.Extensions["traceId"] = context.TraceIdentifier;
    return problem;
}

public partial class Program;
