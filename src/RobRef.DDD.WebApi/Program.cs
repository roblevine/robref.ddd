using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RobRef.DDD.Application.Users.Services;
using RobRef.DDD.Infrastructure.Configuration;
using RobRef.DDD.WebApi.Infrastructure;
using RobRef.DDD.WebApi.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RobRef.DDD User API",
        Version = "v1",
        Description = "API endpoints for user registration."
    });

    options.SupportNonNullableReferenceTypes();
    options.SchemaFilter<RequiredMembersSchemaFilter>();
});

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddInfrastructure();
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServer");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'SqlServer' is not configured.");
    }

    builder.Services.AddInfrastructureWithEfCore(connectionString);
}

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health")
    .WithTags("System")
    .Produces(StatusCodes.Status200OK)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Health check";
        operation.Description = "Returns 200 OK when the API is responsive.";
        return operation;
    });

app.MapPost("/api/users/register", async (
        RegisterUserRequest request,
        UserApplicationService userService,
        CancellationToken cancellationToken) =>
    {
        var userId = await userService.RegisterUserAsync(
            request.Email!,
            string.IsNullOrWhiteSpace(request.Title) ? null : request.Title,
            request.FirstName!,
            request.LastName!,
            cancellationToken);

        var response = new RegisterUserResponse(userId.ToString());
        return Results.Created($"/api/users/{response.Id}", response);
    })
    .AddEndpointFilter(new ValidationEndpointFilter<RegisterUserRequest>())
    .WithName("RegisterUser")
    .WithTags("Users")
    .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Register a new user";
        operation.Description = "Creates a new user using the domain-driven registration flow.";

        SetResponseExample(operation, StatusCodes.Status201Created, UsersExamples.RegisterSuccess, "User registered successfully.", "application/json");
        SetResponseExample(operation, StatusCodes.Status400BadRequest, UsersExamples.ValidationProblem, "Request validation failed.", "application/problem+json");
        SetResponseExample(operation, StatusCodes.Status409Conflict, UsersExamples.DuplicateProblem, "User already exists.", "application/problem+json");

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Example = UsersExamples.RegisterRequest
                }
            }
        };

        return operation;
    });

app.MapGet("/api/users", async (
        UserApplicationService userService,
        CancellationToken cancellationToken) =>
    {
        var users = await userService.GetAllUsersAsync(cancellationToken);
        var userResponses = users.Select(UserResponse.FromDomain).ToList();
        var response = new GetAllUsersResponse(userResponses);
        return Results.Ok(response);
    })
    .WithName("GetAllUsers")
    .WithTags("Users")
    .Produces<GetAllUsersResponse>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Get all users";
        operation.Description = "Retrieves a list of all registered users.";
        return operation;
    });

app.MapGet("/api/users/by-email", async (
        [AsParameters] GetUserByEmailRequest request,
        UserApplicationService userService,
        CancellationToken cancellationToken) =>
    {
        var user = await userService.GetUserByEmailAsync(request.Email, cancellationToken);
        var userResponse = user != null ? UserResponse.FromDomain(user) : null;
        var response = new GetUserByEmailResponse(userResponse);

        return user != null ? Results.Ok(response) : Results.NotFound(response);
    })
    .AddEndpointFilter(new ValidationEndpointFilter<GetUserByEmailRequest>())
    .WithName("GetUserByEmail")
    .WithTags("Users")
    .Produces<GetUserByEmailResponse>(StatusCodes.Status200OK)
    .Produces<GetUserByEmailResponse>(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Get user by email";
        operation.Description = "Retrieves a user by their email address.";
        return operation;
    });

app.Run();

static void SetResponseExample(OpenApiOperation operation, int statusCode, IOpenApiAny example, string description, string contentType)
{
    var key = statusCode.ToString();
    if (!operation.Responses.TryGetValue(key, out var response))
    {
        response = new OpenApiResponse();
        operation.Responses[key] = response;
    }

    response.Description = description;

    response.Content ??= new Dictionary<string, OpenApiMediaType>();
    if (!response.Content.TryGetValue(contentType, out var mediaType))
    {
        mediaType = new OpenApiMediaType();
        response.Content[contentType] = mediaType;
    }

    mediaType.Example = example;
}

public partial class Program;
