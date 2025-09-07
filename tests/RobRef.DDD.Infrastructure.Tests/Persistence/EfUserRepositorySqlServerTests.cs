using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RobRef.DDD.Infrastructure.Persistence;
using System.Text.Json;

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
        var config = SqlServerFixture.GetConfiguration();
        var databaseName = $"{config.TestDatabasePrefix}{Guid.NewGuid():N}";
        var connectionString = $"{config.ConnectionString};Database={databaseName}";
        
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
/// Configuration model for SQL Server test settings.
/// </summary>
public class SqlServerTestConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string TestDatabasePrefix { get; set; } = string.Empty;
}

/// <summary>
/// Fixture that ensures SQL Server is available before running tests.
/// Skips all SQL Server tests if Docker SQL Server container is not running.
/// </summary>
public class SqlServerFixture : IDisposable
{
    private static readonly Lazy<SqlServerTestConfig> _config = new(LoadConfiguration);
    
    public static bool IsSqlServerAvailable { get; private set; }
    public static string SkipReason { get; private set; } = string.Empty;

    static SqlServerFixture()
    {
        try
        {
            var config = GetConfiguration();
            using var connection = new SqlConnection(config.ConnectionString);
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

    public static SqlServerTestConfig GetConfiguration() => _config.Value;

    private static SqlServerTestConfig LoadConfiguration()
    {
        try
        {
            var testDirectory = Path.GetDirectoryName(typeof(SqlServerFixture).Assembly.Location)!;
            var configPath = Path.Combine(testDirectory, "testsettings.local.json");
            
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                var fullConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                if (fullConfig?.TryGetValue("SqlServer", out var sqlServerSection) == true)
                {
                    var sqlServerJson = JsonSerializer.Serialize(sqlServerSection);

                    var ret = JsonSerializer.Deserialize<SqlServerTestConfig>(sqlServerJson);
                    
                    if (ret is null) 
                        throw new InvalidOperationException("Failed to deserialize SqlServer configuration section.");

                    return ret;
                }
            }
            
            // Fallback to defaults
            return new SqlServerTestConfig
            {
                ConnectionString = "Server=localhost,1433;User Id=SA;Password=DevPassword123!;TrustServerCertificate=true;Connection Timeout=5;",
                TestDatabasePrefix = "RobRefDDD_Test_"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            return new SqlServerTestConfig
            {
                ConnectionString = "Server=localhost,1433;User Id=SA;Password=DevPassword123!;TrustServerCertificate=true;Connection Timeout=5;",
                TestDatabasePrefix = "RobRefDDD_Test_"
            };
        }
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}