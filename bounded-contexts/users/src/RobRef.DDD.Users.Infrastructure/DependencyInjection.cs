using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Users.Domain.Users;
using RobRef.DDD.Users.Infrastructure.Persistence;

namespace RobRef.DDD.Users.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers SQL Server-backed infrastructure services.
    /// </summary>
    public static IServiceCollection AddUsersInfrastructureSqlServer(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string must be provided.", nameof(connectionString));
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, EfUserRepository>();

        return services;
    }

    /// <summary>
    /// Registers in-memory infrastructure services (typically for testing).
    /// </summary>
    public static IServiceCollection AddUsersInfrastructureInMemory(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        return services;
    }
}
