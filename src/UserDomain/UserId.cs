using System;

namespace UserDomain;

/// <summary>
/// Strongly-typed identifier for User aggregate. Wraps a Guid for now (future ULID migration possible).
/// </summary>
public readonly struct UserId : IEquatable<UserId>
{
    public Guid Value { get; }

    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId New() => new(Guid.NewGuid());

    public static bool TryParse(string? raw, out UserId id)
    {
        id = default;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        if (!Guid.TryParse(raw, out var g) || g == Guid.Empty) return false;
        id = new UserId(g);
        return true;
    }

    public static UserId Parse(string raw)
    {
        return TryParse(raw, out var id)
            ? id
            : throw new FormatException("Invalid UserId GUID format.");
    }

    public override string ToString() => Value.ToString();
    public bool Equals(UserId other) => Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is UserId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(UserId left, UserId right) => left.Equals(right);
    public static bool operator !=(UserId left, UserId right) => !left.Equals(right);
}
