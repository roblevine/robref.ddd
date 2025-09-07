using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Application.Users.Commands;
using RobRef.DDD.Application.Users.Services;
using RobRef.DDD.Domain.Users;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.Infrastructure.Configuration;

public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services with in-memory repository (for testing/development)
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repository implementations (in-memory)
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        // Application services
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<UserApplicationService>();

        return services;
    }

    /// <summary>
    /// Adds Infrastructure services with EF Core SQL Server (for production)
    /// </summary>
    public static IServiceCollection AddInfrastructureWithEfCore(this IServiceCollection services, string connectionString)
    {
        // EF Core DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repository implementations (EF Core)
        services.AddScoped<IUserRepository, EfUserRepository>();

        // Application services
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<UserApplicationService>();

        return services;
    }
}