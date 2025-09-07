namespace RobRef.DDD.Domain.Users;

public record FirstName
{
    public string Value { get; }

    public FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("First name cannot be null or empty.", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < MinLength)
            throw new ArgumentException($"First name must be at least {MinLength} character long.", nameof(value));

        if (trimmed.Length > MaxLength)
            throw new ArgumentException($"First name cannot exceed {MaxLength} characters.", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;

    public static implicit operator string(FirstName firstName) => firstName.Value;
    public static implicit operator FirstName(string value) => new(value);

    public const int MinLength = 1;
    public const int MaxLength = 50;
}