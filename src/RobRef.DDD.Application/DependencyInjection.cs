using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Application.Users.Commands;
using RobRef.DDD.Application.Users.Queries;
using RobRef.DDD.Application.Users.Services;

namespace RobRef.DDD.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers application-layer services for the user bounded context.
    /// </summary>
    public static IServiceCollection AddUserApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<GetAllUsersHandler>();
        services.AddScoped<GetUserByEmailHandler>();
        services.AddScoped<UserApplicationService>();

        return services;
    }
}
