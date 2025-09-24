using Microsoft.EntityFrameworkCore;
using RobRef.DDD.Users.Domain.Users;

namespace RobRef.DDD.Users.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}