namespace RobRef.DDD.Domain.Users;

public record LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < 1)
            throw new ArgumentException("Last name must be at least 1 character long.", nameof(value));

        if (trimmed.Length > 50)
            throw new ArgumentException("Last name cannot exceed 50 characters.", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;

    public static implicit operator string(LastName lastName) => lastName.Value;
    public static implicit operator LastName(string value) => new(value);
}