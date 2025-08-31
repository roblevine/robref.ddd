using System;
using System.Globalization;
using System.Text;

namespace UserDomain;

/// <summary>
/// First name value object. Case-insensitive equality.
/// </summary>
public sealed record class FirstName : IEquatable<FirstName>
{
    private const int MaxLength = 100;
    public string Value { get; }
    private FirstName(string value) => Value = value;

    public static FirstName Create(string raw)
    {
        if (!TryCreate(raw, out var fn)) throw new ArgumentException("Invalid first name", nameof(raw));
        return fn!;
    }

    public static bool TryCreate(string? raw, out FirstName? firstName)
    {
        firstName = null;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        var norm = CollapseSpaces(raw.Trim());
        if (norm.Length == 0 || norm.Length > MaxLength) return false;
        if (!IsValid(norm)) return false;
        firstName = new FirstName(norm);
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

    public bool Equals(FirstName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => Value.ToUpperInvariant().GetHashCode();
}