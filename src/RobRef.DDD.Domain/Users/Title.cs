namespace RobRef.DDD.Domain.Users;

public record Title
{
    public string? Value { get; }

    public Title(string? value)
    {
        if (value is not null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty or whitespace. Use null for no title.", nameof(value));

            var trimmed = value.Trim();
            if (trimmed.Length < MinLength)
                throw new ArgumentException($"Title must be at least {MinLength} characters long.", nameof(value));

            if (trimmed.Length > MaxLength)
                throw new ArgumentException($"Title cannot exceed {MaxLength} characters.", nameof(value));

            Value = trimmed;
        }
        else
        {
            Value = null;
        }
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator string?(Title title) => title.Value;
    public static implicit operator Title(string? value) => new(value);

    public const int MinLength = 1;
    public const int MaxLength = 20;
}