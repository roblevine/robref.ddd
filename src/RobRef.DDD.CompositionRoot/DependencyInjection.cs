using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Application.Users.Commands;
using RobRef.DDD.Application.Users.Queries;
using RobRef.DDD.Application.Users.Services;
using RobRef.DDD.Domain.Users;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.CompositionRoot;

public static class DependencyInjection
{
    /// <summary>
    /// Registers in-memory infrastructure components primarily used for testing environments.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<GetAllUsersHandler>();
        services.AddScoped<GetUserByEmailHandler>();
        services.AddScoped<UserApplicationService>();

        return services;
    }

    /// <summary>
    /// Registers SQL Server backed infrastructure components intended for production scenarios.
    /// </summary>
    public static IServiceCollection AddInfrastructureWithEfCore(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, EfUserRepository>();

        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<GetAllUsersHandler>();
        services.AddScoped<GetUserByEmailHandler>();
        services.AddScoped<UserApplicationService>();

        return services;
    }
}
