using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Clients;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Testing Testcontainers connectivity...");

        try
        {
            // Test basic Docker connectivity
            var client = new TestcontainersClient();
            var dockerInfo = await client.GetSystemInfoAsync();
            Console.WriteLine($"Docker connected successfully! Version: {dockerInfo.ServerVersion}");

            // Test SQL Server container creation
            var container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("TestPassword123!")
                .WithCleanUp(true)
                .Build();

            Console.WriteLine("Starting container...");
            await container.StartAsync();
            Console.WriteLine($"Container started! Connection string: {container.GetConnectionString()}");

            await container.DisposeAsync();
            Console.WriteLine("Container disposed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
        }
    }
}