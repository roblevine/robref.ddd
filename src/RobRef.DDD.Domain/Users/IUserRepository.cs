namespace RobRef.DDD.Domain.Users;

public interface IUserRepository
{
    // Find operations
    Task<User?> FindByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);

    // Persistence operations
    Task SaveAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    // Collection operations
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> FindByNameAsync(FirstName firstName, LastName lastName, CancellationToken cancellationToken = default);
}