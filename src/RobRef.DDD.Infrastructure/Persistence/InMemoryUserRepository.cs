using System;
using RobRef.DDD.Domain.Users;
using System.Collections.Concurrent;
using System.Linq;

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
        var user = _users.Values.FirstOrDefault(u => string.Equals(u.Email.Value, email.Value, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_users.ContainsKey(id));
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var exists = _users.Values.Any(u => string.Equals(u.Email.Value, email.Value, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var emailTaken = _users.Values.Any(existing => existing.Id != user.Id && string.Equals(existing.Email.Value, user.Email.Value, StringComparison.OrdinalIgnoreCase));
        if (emailTaken)
        {
            throw new InvalidOperationException($"A user with email '{user.Email.Value}' already exists.");
        }

        _users.AddOrUpdate(
            user.Id,
            user,
            (_, existing) =>
            {
                existing.ChangeEmail(user.Email);
                existing.ChangeName(user.Title, user.FirstName, user.LastName);
                return existing;
            });

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