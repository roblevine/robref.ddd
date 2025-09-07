using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.Infrastructure.Tests.Persistence;

/// <summary>
/// SQL Server database implementation of repository integration tests.
/// Runs all tests from the base class using real SQL Server database.
/// Tests are conditionally skipped if SQL Server is not available.
/// </summary>
[Collection("SqlServer")]
public class EfUserRepositorySqlServerTests : UserRepositoryIntegrationTestsBase
{
    public EfUserRepositorySqlServerTests() : base(
        SqlServerFixture.IsSqlServerAvailable ? CreateSqlServerOptions() : null!,
        !SqlServerFixture.IsSqlServerAvailable,
        SqlServerFixture.SkipReason)
    {
    }

    private static DbContextOptions<ApplicationDbContext> CreateSqlServerOptions()
    {
        // Create a unique database for each test class instance
        var databaseName = $"RobRefDDD_Test_{Guid.NewGuid():N}";
        var connectionString = $"Server=localhost,1433;Database={databaseName};User Id=SA;Password=DevPassword123!;TrustServerCertificate=true;";
        
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;
    }

    public override void Dispose()
    {
        try
        {
            // Clean up the test database
            Context.Database.EnsureDeleted();
        }
        catch
        {
            // Ignore cleanup errors
        }
        finally
        {
            base.Dispose();
        }
    }
}

/// <summary>
/// Collection definition for SQL Server tests with conditional execution.
/// </summary>
[CollectionDefinition("SqlServer")]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
}

/// <summary>
/// Fixture that ensures SQL Server is available before running tests.
/// Skips all SQL Server tests if Docker SQL Server container is not running.
/// </summary>
public class SqlServerFixture : IDisposable
{
    public static bool IsSqlServerAvailable { get; private set; }
    public static string SkipReason { get; private set; } = string.Empty;

    static SqlServerFixture()
    {
        try
        {
            using var connection = new SqlConnection("Server=localhost,1433;User Id=SA;Password=DevPassword123!;TrustServerCertificate=true;Connection Timeout=5;");
            connection.Open();
            
            // Test that we can execute a simple query
            using var command = new SqlCommand("SELECT 1", connection);
            command.ExecuteScalar();
            
            IsSqlServerAvailable = true;
        }
        catch (Exception ex)
        {
            IsSqlServerAvailable = false;
            SkipReason = $"SQL Server not available - run 'docker-compose up -d sqlserver' to enable SQL Server tests. Error: {ex.Message}";
            
            // Log for debugging
            Console.WriteLine($"SQL Server not available: {ex.Message}");
        }
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}