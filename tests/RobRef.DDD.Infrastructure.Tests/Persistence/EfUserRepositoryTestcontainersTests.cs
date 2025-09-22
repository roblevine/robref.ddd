using Microsoft.EntityFrameworkCore;
using RobRef.DDD.Infrastructure.Persistence;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Data.SqlClient;

namespace RobRef.DDD.Infrastructure.Tests.Persistence;

/// <summary>
/// Testcontainers database implementation of repository integration tests.
/// Runs all tests from the base class using Testcontainers-managed SQL Server.
/// Each test class gets its own isolated container instance.
/// </summary>
[Collection("Testcontainers")]
[Trait("Database", "Testcontainers")]
public class EfUserRepositoryTestcontainersTests : UserRepositoryIntegrationTestsBase
{
    private readonly TestcontainersFixture _fixture;

    public EfUserRepositoryTestcontainersTests(TestcontainersFixture fixture) : base(
        CreateTestcontainersOptions(fixture),
        false,
        string.Empty)
    {
        _fixture = fixture;
    }

    private static DbContextOptions<ApplicationDbContext> CreateTestcontainersOptions(TestcontainersFixture fixture)
    {
        var databaseName = $"RobRefDDD_Test_{Guid.NewGuid():N}";
        var baseConnectionString = fixture.GetConnectionString();

        // Ensure proper connection string format
        var connectionString = baseConnectionString.Contains("Database=")
            ? baseConnectionString.Replace("Database=master", $"Database={databaseName}")
            : $"{baseConnectionString};Database={databaseName}";

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
/// Collection definition for Testcontainers tests.
/// </summary>
[CollectionDefinition("Testcontainers")]
public class TestcontainersCollection : ICollectionFixture<TestcontainersFixture>
{
}

/// <summary>
/// Fixture that manages the Testcontainers SQL Server instance.
/// Container is shared across all tests in the collection for performance.
/// </summary>
public class TestcontainersFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    private string? _workingConnectionString;

    public TestcontainersFixture()
    {
        TestcontainersSettings.ResourceReaperEnabled = false;
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("TestPassword123!")
            .WithPortBinding(0, 1433) // Use random available port
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(1433)
                .UntilMessageIsLogged("SQL Server is now ready for client connections"))
            .Build();
    }

    public string GetConnectionString() => _workingConnectionString ?? _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        try
        {
            Console.WriteLine("Starting testcontainer SQL Server on random port...");
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(3)).Token;
            await _container.StartAsync(cancellationToken);

            var port = _container.GetMappedPublicPort(1433);
            var originalConnectionString = _container.GetConnectionString();
            Console.WriteLine($"Testcontainer started successfully on port {port}: {originalConnectionString}");

            // Give SQL Server some time to fully initialize
            Console.WriteLine("Waiting 10 seconds for SQL Server to fully initialize...");
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            // Validate connection with retries
            await ValidateConnectionAsync(cancellationToken);
            Console.WriteLine("SQL Server connection validated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start testcontainer: {ex.Message}");
            throw new InvalidOperationException(
                "Failed to start Testcontainers SQL Server container. " +
                $"Original error: {ex.Message}", ex);
        }
    }

    private async Task ValidateConnectionAsync(CancellationToken cancellationToken)
    {
        var originalConnectionString = _container.GetConnectionString();
        var port = _container.GetMappedPublicPort(1433);

        // Try multiple connection string formats, prioritizing host.docker.internal
        var connectionStrings = new[]
        {
            $"Server=host.docker.internal,{port};Database=master;User Id=sa;Password=TestPassword123!;TrustServerCertificate=True;Connection Timeout=30;",
            $"Server=localhost,{port};Database=master;User Id=sa;Password=TestPassword123!;TrustServerCertificate=True;Connection Timeout=30;",
            originalConnectionString,
            $"Server=127.0.0.1,{port};Database=master;User Id=sa;Password=TestPassword123!;TrustServerCertificate=True;Connection Timeout=30;"
        };

        var maxRetries = 10;
        var delay = TimeSpan.FromSeconds(3);

        foreach (var connectionString in connectionStrings)
        {
            Console.WriteLine($"Trying connection string: {connectionString}");

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using var connection = new SqlConnection(connectionString);
                    Console.WriteLine($"Attempt {i + 1}: Opening connection...");
                    await connection.OpenAsync(cancellationToken);

                    Console.WriteLine($"Attempt {i + 1}: Connection opened, executing test query...");
                    using var command = new SqlCommand("SELECT 1", connection);
                    command.CommandTimeout = 15;
                    var result = await command.ExecuteScalarAsync(cancellationToken);

                    Console.WriteLine($"Connection validated successfully on attempt {i + 1}, result: {result}");
                    Console.WriteLine($"Working connection string: {connectionString}");
                    _workingConnectionString = connectionString;
                    return;
                }
                catch (Exception ex) when (i < maxRetries - 1)
                {
                    Console.WriteLine($"Connection attempt {i + 1} failed: {ex.GetType().Name}: {ex.Message}");
                    await Task.Delay(delay, cancellationToken);
                }
                catch (Exception ex) when (i == maxRetries - 1)
                {
                    Console.WriteLine($"Final attempt {i + 1} failed for this connection string: {ex.GetType().Name}: {ex.Message}");
                    break; // Try next connection string
                }
            }
        }

        throw new InvalidOperationException($"Failed to validate connection with any connection string after {maxRetries} attempts each. Original: {originalConnectionString}");
    }

    public async Task DisposeAsync()
    {
        try
        {
            Console.WriteLine("Disposing testcontainer...");
            await _container.DisposeAsync();
            Console.WriteLine("Testcontainer disposed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disposing testcontainer: {ex.Message}");
            // Don't throw from dispose
        }
    }
}
