using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using RobRef.DDD.Infrastructure.Persistence;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

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
    private MsSqlContainer _container;
    private string? _workingConnectionString;
    private static readonly object SettingsGate = new();
    private static bool _settingsConfigured;
    private bool _resourceReaperEnabled = true;
    private const string ContainerLabelKey = "robref.ddd.testcontainers";
    private const string ContainerLabelValue = "true";

    public TestcontainersFixture()
    {
        ConfigureTestcontainersSettings();
        _resourceReaperEnabled = TestcontainersSettings.ResourceReaperEnabled;
        _container = CreateContainer();
    }

    private static MsSqlContainer CreateContainer()
    {
        return new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("TestPassword123!")
            .WithPortBinding(0, 1433) // Use random available port
            .WithLabel(ContainerLabelKey, ContainerLabelValue)
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(1433)
                .UntilMessageIsLogged("SQL Server is now ready for client connections"))
            .Build();
    }

    private static void ConfigureTestcontainersSettings()
    {
        if (_settingsConfigured)
        {
            return;
        }

        lock (SettingsGate)
        {
            if (_settingsConfigured)
            {
                return;
            }

            var runningInContainer = string.Equals(
                Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                "true",
                StringComparison.OrdinalIgnoreCase)
                || File.Exists("/.dockerenv");

            if (runningInContainer)
            {
                Console.WriteLine("Detected containerised test run; disabling Testcontainers resource reaper.");
                TestcontainersSettings.ResourceReaperEnabled = false;
                TestcontainersSettings.ResourceReaperPrivilegedModeEnabled = false;

                if (File.Exists("/var/run/docker.sock"))
                {
                    TestcontainersSettings.DockerSocketOverride = "/var/run/docker.sock";
                }

                var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST");
                if (!string.IsNullOrWhiteSpace(dockerHost))
                {
                    TestcontainersSettings.DockerHostOverride = dockerHost;
                }
            }
            else
            {
                TestcontainersSettings.ResourceReaperEnabled = true;
                TestcontainersSettings.ResourceReaperPrivilegedModeEnabled = false;
                TestcontainersSettings.DockerSocketOverride = null;
                TestcontainersSettings.DockerHostOverride = null;
            }

            _settingsConfigured = true;
        }
    }

    private static void DisableResourceReaper()
    {
        Console.WriteLine("Disabling Testcontainers resource reaper; falling back to manual cleanup.");
        TestcontainersSettings.ResourceReaperEnabled = false;
        TestcontainersSettings.ResourceReaperPrivilegedModeEnabled = false;
        TestcontainersSettings.DockerSocketOverride = null;
        TestcontainersSettings.DockerHostOverride = null;
    }

    private static bool IsResourceReaperBootstrapFailure(Exception ex)
    {
        var diagnostic = ex.ToString();
        return diagnostic.Contains("DockerContainerNotFoundException", StringComparison.OrdinalIgnoreCase)
            || diagnostic.Contains("Resource Reaper", StringComparison.OrdinalIgnoreCase)
            || diagnostic.Contains("ResourceReaper", StringComparison.OrdinalIgnoreCase);
    }

    private async Task DisposeContainerSilentlyAsync()
    {
        try
        {
            await _container.DisposeAsync();
        }
        catch (Exception disposeEx)
        {
            Console.WriteLine($"Cleanup during resource reaper fallback failed: {disposeEx.Message}");
        }
    }

    private async Task StartContainerAsync()
    {
        Console.WriteLine("Starting testcontainer SQL Server on random port...");
        using var cancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        var cancellationToken = cancellationSource.Token;

        await _container.StartAsync(cancellationToken);

        var port = _container.GetMappedPublicPort(1433);
        var originalConnectionString = _container.GetConnectionString();
        Console.WriteLine($"Testcontainer started successfully on port {port}: {originalConnectionString}");

        Console.WriteLine("Waiting 10 seconds for SQL Server to fully initialize...");
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        await ValidateConnectionAsync(cancellationToken);
        Console.WriteLine("SQL Server connection validated successfully");
    }

    private static async Task CleanupDanglingContainersAsync()
    {
        try
        {
            var labelFilter = $"label={ContainerLabelKey}={ContainerLabelValue}";
            var (exitCode, stdout, stderr) = await RunDockerCommandAsync("ps", "-aq", "-f", labelFilter);
            if (exitCode != 0)
            {
                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    Console.WriteLine($"docker ps for cleanup failed: {stderr.Trim()}");
                }
                return;
            }

            var containerIds = stdout
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (containerIds.Length == 0)
            {
                return;
            }

            var arguments = new List<string> { "rm", "-f" };
            arguments.AddRange(containerIds);
            var (rmExitCode, _, rmStderr) = await RunDockerCommandAsync(arguments.ToArray());
            if (rmExitCode != 0 && !string.IsNullOrWhiteSpace(rmStderr))
            {
                Console.WriteLine($"docker rm for cleanup failed: {rmStderr.Trim()}");
            }
            else
            {
                Console.WriteLine($"Pruned lingering test containers: {string.Join(", ", containerIds)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to prune lingering test containers: {ex.Message}");
        }
    }

    private static async Task<(int exitCode, string stdout, string stderr)> RunDockerCommandAsync(params string[] args)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            foreach (var argument in args)
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return (-1, string.Empty, "Failed to start docker process.");
            }

            var stdOutTask = process.StandardOutput.ReadToEndAsync();
            var stdErrTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            var stdout = await stdOutTask;
            var stderr = await stdErrTask;
            return (process.ExitCode, stdout, stderr);
        }
        catch (Exception ex)
        {
            return (-1, string.Empty, ex.Message);
        }
    }

    public string GetConnectionString() => _workingConnectionString ?? _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        try
        {
            await StartContainerAsync();
        }
        catch (Exception ex) when (IsResourceReaperBootstrapFailure(ex) && _resourceReaperEnabled)
        {
            Console.WriteLine($"Resource reaper failed to start ({ex.Message}). Retrying without reaper...");
            await DisposeContainerSilentlyAsync();
            await CleanupDanglingContainersAsync();
            DisableResourceReaper();
            _resourceReaperEnabled = false;
            _container = CreateContainer();
            _workingConnectionString = null;
            await StartContainerAsync();
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
        finally
        {
            await CleanupDanglingContainersAsync();
        }
    }
}

