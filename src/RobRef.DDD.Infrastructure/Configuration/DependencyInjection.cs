using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Application.Users.Commands;
using RobRef.DDD.Application.Users.Services;
using RobRef.DDD.Domain.Users;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repository implementations
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        // Application services
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<UserApplicationService>();

        return services;
    }
}