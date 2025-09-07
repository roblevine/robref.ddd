using RobRef.DDD.Application.Users.Commands;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Services;

public sealed class UserApplicationService
{
    private readonly RegisterUserHandler _registerUserHandler;

    public UserApplicationService(RegisterUserHandler registerUserHandler)
    {
        _registerUserHandler = registerUserHandler ?? throw new ArgumentNullException(nameof(registerUserHandler));
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
}