using RobRef.DDD.Application.Users.Commands;
using RobRef.DDD.Application.Users.Queries;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Services;

public sealed class UserApplicationService
{
    private readonly RegisterUserHandler _registerUserHandler;
    private readonly GetAllUsersHandler _getAllUsersHandler;
    private readonly GetUserByEmailHandler _getUserByEmailHandler;

    public UserApplicationService(
        RegisterUserHandler registerUserHandler,
        GetAllUsersHandler getAllUsersHandler,
        GetUserByEmailHandler getUserByEmailHandler)
    {
        _registerUserHandler = registerUserHandler ?? throw new ArgumentNullException(nameof(registerUserHandler));
        _getAllUsersHandler = getAllUsersHandler ?? throw new ArgumentNullException(nameof(getAllUsersHandler));
        _getUserByEmailHandler = getUserByEmailHandler ?? throw new ArgumentNullException(nameof(getUserByEmailHandler));
    }

    public async Task<UserId> RegisterUserAsync(
        string email,
        string? title,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUser(email, title, firstName, lastName);
        return await _registerUserHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsers();
        return await _getAllUsersHandler.HandleAsync(query, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetUserByEmail(email);
        return await _getUserByEmailHandler.HandleAsync(query, cancellationToken);
    }
}