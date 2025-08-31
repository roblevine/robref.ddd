using System;
using System.Globalization;
using System.Text;

namespace UserDomain;

/// <summary>
/// Last name value object. Case-insensitive equality.
/// </summary>
public sealed record class LastName : IEquatable<LastName>
{
    private const int MaxLength = 100;
    public string Value { get; }
    private LastName(string value) => Value = value;

    public static LastName Create(string raw)
    {
        if (!TryCreate(raw, out var ln)) throw new ArgumentException("Invalid last name", nameof(raw));
        return ln!;
    }

    public static bool TryCreate(string? raw, out LastName? lastName)
    {
        lastName = null;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        var norm = CollapseSpaces(raw.Trim());
        if (norm.Length == 0 || norm.Length > MaxLength) return false;
        if (!IsValid(norm)) return false;
        lastName = new LastName(norm);
        return true;
    }

    private static bool IsValid(string value)
    {
        foreach (var rune in value.EnumerateRunes())
            if (!NameEqualityExtensions.IsAllowedNameRune(rune)) return false;
        return true;
    }

    private static string CollapseSpaces(string input)
    {
        if (input.IndexOf(' ') == -1) return input;
        var sb = new StringBuilder(input.Length);
        bool prev = false;
        foreach (var ch in input)
        {
            if (ch == ' ')
            {
                if (prev) continue; prev = true; sb.Append(ch);
            }
            else { prev = false; sb.Append(ch); }
        }
        return sb.ToString();
    }

    public override string ToString() => Value;

    public bool Equals(LastName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => Value.ToUpperInvariant().GetHashCode();
}