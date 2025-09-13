using RobRef.DDD.Application.Common;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Application.Users.Commands;

public sealed class RegisterUserHandler(IUserRepository userRepository) : ICommandHandler<RegisterUser, UserId>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public async Task<UserId> HandleAsync(RegisterUser command, CancellationToken cancellationToken = default)
    {
        // Create email value object (validates format)
        var email = new Email(command.Email);

        // Check if user already exists
        var existingUser = await _userRepository.FindByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException($"User with email '{email.Value}' already exists.");
        }

        // Create name value objects
        var title = !string.IsNullOrWhiteSpace(command.Title) ? new Title(command.Title) : null;
        var firstName = new FirstName(command.FirstName);
        var lastName = new LastName(command.LastName);

        // Create user through factory method
        var user = User.Register(email, title, firstName, lastName);

        // Save user
        await _userRepository.SaveAsync(user, cancellationToken);

        return user.Id;
    }
}