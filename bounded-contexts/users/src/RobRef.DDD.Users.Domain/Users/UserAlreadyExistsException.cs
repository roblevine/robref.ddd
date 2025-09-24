namespace RobRef.DDD.Users.Domain.Users;

public sealed class UserAlreadyExistsException : InvalidOperationException
{
    public Email Email { get; }

    public UserAlreadyExistsException(Email email)
        : base($"A user with email '{email.Value}' already exists.")
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
}
