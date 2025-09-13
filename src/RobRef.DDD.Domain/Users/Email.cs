using System.Text.RegularExpressions;

namespace RobRef.DDD.Domain.Users;

public record Email : IComparable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null or empty.", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"Email cannot exceed {MaxLength} characters.", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException($"Invalid email format: {value}", nameof(value));

        Value = value;
    }

    public const int MaxLength = 254; // RFC 5321 maximum length for an email address
    
    public override string ToString() => Value;

    public int CompareTo(Email? other)
    {
        if (other is null) return 1;
        return string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);
}