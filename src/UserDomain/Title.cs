using System;
using System.Globalization;
using System.Text;

namespace UserDomain;

/// <summary>
/// Optional honorific title. Case-insensitive equality; preserves original casing.
/// </summary>
public sealed record class Title : IEquatable<Title>
{
    private const int MaxLength = 30;
    public string Value { get; }
    private Title(string value) => Value = value;

    public static bool TryCreate(string? raw, out Title? title)
    {
        title = null;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        var norm = CollapseSpaces(raw.Trim());
        if (norm.Length == 0 || norm.Length > MaxLength) return false;
        if (!IsValid(norm)) return false;
        title = new Title(norm);
        return true;
    }

    public static Title Create(string raw)
        => TryCreate(raw, out var t) ? t! : throw new ArgumentException("Invalid title", nameof(raw));

    private static bool IsValid(string value)
    {
        foreach (var rune in value.EnumerateRunes())
            if (!NameEqualityExtensions.IsAllowedNameRune(rune, allowPeriod: true)) return false;
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
                if (prev) continue;
                prev = true; sb.Append(ch);
            }
            else { prev = false; sb.Append(ch); }
        }
        return sb.ToString();
    }

    public override string ToString() => Value;

    public bool Equals(Title? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => Value.ToUpperInvariant().GetHashCode();
}