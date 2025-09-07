using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.Infrastructure;

// This is a minimal program to support EF Core migrations
// It will be removed when we create the Web API project in Phase 6
public class Program
{
    public static void Main(string[] args)
    {
        // This is only used for design-time services (migrations)
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer("Server=localhost;Database=RobRefDDD;Trusted_Connection=true;TrustServerCertificate=true;"));
            });
}