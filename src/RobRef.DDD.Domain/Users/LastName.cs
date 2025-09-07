namespace RobRef.DDD.Domain.Users;

public record LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < MinLength)
            throw new ArgumentException($"Last name must be at least {MinLength} character long.", nameof(value));

        if (trimmed.Length > MaxLength)
            throw new ArgumentException($"Last name cannot exceed {MaxLength} characters.", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;

    public static implicit operator string(LastName lastName) => lastName.Value;
    public static implicit operator LastName(string value) => new(value);

    public const int MinLength = 1;
    public const int MaxLength = 50;
}