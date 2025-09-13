using RobRef.DDD.Domain.Users;
using System.Collections.Concurrent;

namespace RobRef.DDD.Infrastructure.Persistence;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<UserId, User> _users = new();

    public Task<User?> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email.Equals(email));
        return Task.FromResult(user);
    }

    public Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_users.ContainsKey(id));
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var exists = _users.Values.Any(u => u.Email.Equals(email));
        return Task.FromResult(exists);
    }

    public Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        _users.AddOrUpdate(user.Id, user, (key, existingValue) => user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        _users.TryRemove(user.Id, out _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<User>>(_users.Values.ToList());
    }

    public Task<IReadOnlyList<User>> FindByNameAsync(FirstName firstName, LastName lastName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        var users = _users.Values
            .Where(u => u.FirstName.Equals(firstName) && u.LastName.Equals(lastName))
            .ToList();
        return Task.FromResult<IReadOnlyList<User>>(users);
    }
}