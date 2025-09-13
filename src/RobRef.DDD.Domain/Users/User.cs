namespace RobRef.DDD.Domain.Users;

public class User : IEquatable<User>
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public Title? Title { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }

    // Private constructor to enforce factory pattern
    private User(UserId id, Email email, Title? title, FirstName firstName, LastName lastName)
    {
        Id = id;
        Email = email;
        Title = title;
        FirstName = firstName;
        LastName = lastName;
    }

    // Factory method for registration
    public static User Register(Email email, Title? title, FirstName firstName, LastName lastName)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        var id = UserId.NewId();
        return new User(id, email, title, firstName, lastName);
    }

    // Factory for reconstruction (e.g., from persistence, testing)
    // NOTE: Use Register() for new user creation in business scenarios
    public static User Create(UserId id, Email email, Title? title, FirstName firstName, LastName lastName)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        return new User(id, email, title, firstName, lastName);
    }

    // Domain behavior methods
    public void ChangeEmail(Email newEmail)
    {
        ArgumentNullException.ThrowIfNull(newEmail);
        Email = newEmail;
    }

    public void ChangeName(Title? title, FirstName firstName, LastName lastName)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        Title = title;
        FirstName = firstName;
        LastName = lastName;
    }

    // Entity equality based on Id
    public bool Equals(User? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as User);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(User? left, User? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(User? left, User? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        var titleStr = Title?.Value != null ? $"{Title} " : "";
        return $"{titleStr}{FirstName} {LastName} ({Email})";
    }
}