using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobRef.DDD.Domain.Users;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.WebApi.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly InMemoryUserRepository _sharedRepository = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            var repositoryDescriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(IUserRepository));
            if (repositoryDescriptor is not null)
            {
                services.Remove(repositoryDescriptor);
            }

            services.AddSingleton<IUserRepository>(_sharedRepository);
        });
    }

    public void ClearRepository()
    {
        // Clear the repository state between tests
        var field = typeof(InMemoryUserRepository).GetField("_users", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field?.GetValue(_sharedRepository) is System.Collections.Concurrent.ConcurrentDictionary<UserId, User> users)
        {
            users.Clear();
        }
    }
}
