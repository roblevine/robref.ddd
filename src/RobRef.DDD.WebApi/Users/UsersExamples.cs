using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;

namespace RobRef.DDD.WebApi.Users;

internal static class UsersExamples
{
    public static IOpenApiAny RegisterRequest { get; } = new OpenApiObject
    {
        ["email"] = new OpenApiString("jane.doe@example.com"),
        ["title"] = new OpenApiString("Ms"),
        ["firstName"] = new OpenApiString("Jane"),
        ["lastName"] = new OpenApiString("Doe")
    };

    public static IOpenApiAny RegisterSuccess { get; } = new OpenApiObject
    {
        ["id"] = new OpenApiString("01HXZ7J6FNT3E8PJ4A8J7Z5WKR")
    };

    public static IOpenApiAny ValidationProblem { get; } = new OpenApiObject
    {
        ["type"] = new OpenApiString("https://robref.ddd/problems/validation-error"),
        ["title"] = new OpenApiString("Validation Failed"),
        ["status"] = new OpenApiInteger(StatusCodes.Status400BadRequest),
        ["errors"] = new OpenApiObject
        {
            ["Email"] = new OpenApiArray
            {
                new OpenApiString("The Email field is not a valid e-mail address.")
            }
        }
    };

    public static IOpenApiAny DuplicateProblem { get; } = new OpenApiObject
    {
        ["type"] = new OpenApiString("https://robref.ddd/problems/user-already-exists"),
        ["title"] = new OpenApiString("User Already Exists"),
        ["status"] = new OpenApiInteger(StatusCodes.Status409Conflict),
        ["detail"] = new OpenApiString("User with email 'jane.doe@example.com' already exists.")
    };
}
