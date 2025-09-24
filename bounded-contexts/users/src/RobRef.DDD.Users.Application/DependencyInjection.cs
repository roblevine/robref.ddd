using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Users.Application.Users.Commands;
using RobRef.DDD.Users.Application.Users.Queries;
using RobRef.DDD.Users.Application.Users.Services;

namespace RobRef.DDD.Users.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers application-layer services for the user bounded context.
    /// </summary>
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<GetAllUsersHandler>();
        services.AddScoped<GetUserByEmailHandler>();
        services.AddScoped<UserApplicationService>();

        return services;
    }
}
