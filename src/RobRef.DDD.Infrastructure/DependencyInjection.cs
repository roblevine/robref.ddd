using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Domain.Users;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers SQL Server-backed infrastructure services.
    /// </summary>
    public static IServiceCollection AddInfrastructureSqlServer(this IServiceCollection services, string connectionString)
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
    public static IServiceCollection AddInfrastructureInMemory(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        return services;
    }
}
