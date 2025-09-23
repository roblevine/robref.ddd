using Microsoft.EntityFrameworkCore;
using System;
using RobRef.DDD.Infrastructure.Persistence;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System.Diagnostics;

namespace RobRef.DDD.Infrastructure.Tests.Persistence;

/// <summary>
/// Testcontainers database implementation of repository integration tests.
/// Runs all tests from the base class using Testcontainers-managed SQL Server.
/// Uses standard TestContainers patterns for automatic container lifecycle management.
/// </summary>
[Collection("Testcontainers")]
[Trait("Database", "Testcontainers")]
public class EfUserRepositoryTestcontainersTests : UserRepositoryIntegrationTestsBase
{
    private readonly TestcontainersFixture _fixture;

    public EfUserRepositoryTestcontainersTests(TestcontainersFixture fixture, ITestOutputHelper outputHelper)
        : base(CreateTestcontainersOptions(fixture))
    {
        _fixture = fixture;
    }

    private static DbContextOptions<ApplicationDbContext> CreateTestcontainersOptions(TestcontainersFixture fixture)
    {
        var databaseName = $"RobRefDDD_Test_{Guid.NewGuid():N}";
        var baseConnectionString = fixture.GetConnectionString();

        // Ensure proper connection string format and add required SQL Server 2022 connection parameters
        var connectionString = baseConnectionString.Contains("Database=")
            ? baseConnectionString.Replace("Database=master", $"Database={databaseName}")
            : $"{baseConnectionString};Database={databaseName}";
            
        // Add Encrypt=false for SQL Server 2022 compatibility in containerized environments
        if (!connectionString.Contains("Encrypt="))
        {
            connectionString += ";Encrypt=false";
        }

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
/// Uses standard TestContainers patterns with environment-based configuration.
/// </summary>
public class TestcontainersFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    private static bool _settingsConfigured;
    private static readonly object SettingsLock = new();

    public TestcontainersFixture()
    {
        ConfigureTestcontainersSettings();
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("TestPassword123!")
            // No port binding - let TestContainers choose a random available port
            .WithCleanUp(true) // Enable automatic cleanup
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(1433) // This refers to the internal container port
                .UntilMessageIsLogged("SQL Server is now ready for client connections"))
            .Build();
    }

    /// <summary>
    /// Configure TestContainers based on standard environment variables.
    /// This follows TestContainers best practices for DooD scenarios.
    /// </summary>
    private static void ConfigureTestcontainersSettings()
    {
        if (_settingsConfigured)
            return;

        lock (SettingsLock)
        {
            if (_settingsConfigured)
                return;

            // Standard TestContainers environment variable for disabling Ryuk
            var ryukDisabled = Environment.GetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED");
            if (string.Equals(ryukDisabled, "true", StringComparison.OrdinalIgnoreCase))
            {
                TestcontainersSettings.ResourceReaperEnabled = false;
            }

            // Standard TestContainers environment variable for host override
            var hostOverride = Environment.GetEnvironmentVariable("TESTCONTAINERS_HOST_OVERRIDE");
            if (!string.IsNullOrEmpty(hostOverride))
            {
                TestcontainersSettings.DockerHostOverride = hostOverride;
            }

            // Legacy: Also check for container detection for backward compatibility
            var runningInContainer = string.Equals(
                Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                "true",
                StringComparison.OrdinalIgnoreCase);

            if (runningInContainer && ryukDisabled != "true")
            {
                TestcontainersSettings.ResourceReaperEnabled = false;
            }

            _settingsConfigured = true;
        }
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        // Brief delay to ensure SQL Server is fully ready
        await Task.Delay(2000);
    }

    public async Task DisposeAsync()
    {
        try
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
            
            // Manual cleanup for dev container environment where Ryuk is disabled
            var containerId = _container.Id;
            
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"rm -f {containerId}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            // Log cleanup errors but don't fail tests
            Console.WriteLine($"TestContainer cleanup error: {ex.Message}");
        }
    }    public string GetConnectionString() => _container.GetConnectionString();
}

