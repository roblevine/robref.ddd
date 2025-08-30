using System;

namespace AcmeStore.Domain.Users.ValueObjects;

/// <summary>
/// Strongly-typed user identifier value object backed by a Guid.
/// </summary>
public readonly record struct UserId
{
    public Guid Value { get; }

    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId New()
    {
        return new UserId(Guid.NewGuid());
    }

    public static UserId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(value));
        }
        return new UserId(value);
    }

    public static UserId Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("UserId string cannot be null or whitespace.", nameof(input));
        }

        if (!Guid.TryParse(input, out var guid))
        {
            throw new ArgumentException("Invalid UserId format.", nameof(input));
        }

        return From(guid);
    }

    public static bool TryParse(string? input, out UserId userId)
    {
        userId = default;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        if (!Guid.TryParse(input, out var guid) || guid == Guid.Empty)
        {
            return false;
        }

        userId = new UserId(guid);
        return true;
    }

    public override string ToString() => Value.ToString();
}


