namespace RobRef.DDD.Domain.Users;

public record FirstName
{
    public string Value { get; }

    public FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("First name cannot be null or empty.", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < 1)
            throw new ArgumentException("First name must be at least 1 character long.", nameof(value));

        if (trimmed.Length > 50)
            throw new ArgumentException("First name cannot exceed 50 characters.", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;

    public static implicit operator string(FirstName firstName) => firstName.Value;
    public static implicit operator FirstName(string value) => new(value);
}