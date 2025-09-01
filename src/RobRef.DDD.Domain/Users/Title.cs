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
            if (trimmed.Length < 2)
                throw new ArgumentException("Title must be at least 2 characters long.", nameof(value));

            if (trimmed.Length > 20)
                throw new ArgumentException("Title cannot exceed 20 characters.", nameof(value));

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
}