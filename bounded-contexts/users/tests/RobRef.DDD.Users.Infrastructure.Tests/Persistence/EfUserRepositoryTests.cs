using Microsoft.EntityFrameworkCore;
using RobRef.DDD.Users.Infrastructure.Persistence;

namespace RobRef.DDD.Users.Infrastructure.Tests.Persistence;

/// <summary>
/// InMemory database implementation of repository integration tests.
/// Runs all tests from the base class using EF Core InMemory provider.
/// </summary>
public class EfUserRepositoryInMemoryTests : UserRepositoryIntegrationTestsBase
{
    public EfUserRepositoryInMemoryTests() : base(CreateInMemoryOptions())
    {
    }

    private static DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
}