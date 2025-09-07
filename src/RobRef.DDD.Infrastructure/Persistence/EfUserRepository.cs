using Microsoft.EntityFrameworkCore;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Infrastructure.Persistence;

public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public EfUserRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (existingUser == null)
        {
            // Check for duplicate email before adding
            var emailExists = await ExistsByEmailAsync(user.Email, cancellationToken);
            if (emailExists)
            {
                throw new InvalidOperationException($"A user with email '{user.Email.Value}' already exists.");
            }
            
            // Add new user
            _context.Users.Add(user);
        }
        else
        {
            // Check for duplicate email when updating (excluding current user)
            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email == user.Email && u.Id != user.Id, cancellationToken);
            if (emailTaken)
            {
                throw new InvalidOperationException($"A user with email '{user.Email.Value}' already exists.");
            }

            // Update existing user properties
            existingUser.ChangeEmail(user.Email);
            existingUser.ChangeName(user.Title, user.FirstName, user.LastName);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (existingUser != null)
        {
            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);

        return users.AsReadOnly();
    }

    public async Task<IReadOnlyList<User>> FindByNameAsync(FirstName firstName, LastName lastName, CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .Where(u => u.FirstName == firstName && u.LastName == lastName)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return users.AsReadOnly();
    }
}