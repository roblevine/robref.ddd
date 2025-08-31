using System;
using System.Linq;

namespace UserDomain;

/// <summary>
/// Email address value object. Canonical form is trimmed & lowercased.
/// </summary>
public sealed record class EmailAddress
{
    private static readonly char[] ForbiddenWhitespace = new[] { ' ', '\t', '\r', '\n' };
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) throw new ArgumentException("Email cannot be empty", nameof(raw));
        var normalized = raw.Trim().ToLowerInvariant();
        if (!IsValid(normalized)) throw new ArgumentException("Invalid email format", nameof(raw));
        return new EmailAddress(normalized);
    }

    public static bool TryParse(string? raw, out EmailAddress email)
    {
        email = null!;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        var normalized = raw.Trim().ToLowerInvariant();
        if (!IsValid(normalized)) return false;
        email = new EmailAddress(normalized);
        return true;
    }

    private static bool IsValid(string value)
    {
        // Lightweight pragmatic validation (avoid complex regex for now)
        // 1. Must contain exactly one '@'
        var atIndex = value.IndexOf('@');
        if (atIndex <= 0) return false; // cannot start with @ and need local part
        if (value.IndexOf('@', atIndex + 1) != -1) return false; // second '@'

        // 2. Domain part must contain at least one '.' not at start/end
        var domain = value[(atIndex + 1)..];
        if (domain.Length < 3) return false; // need something like a.b
        if (domain.StartsWith('.') || domain.EndsWith('.')) return false;
        if (!domain.Contains('.')) return false;

        // 3. No whitespace inside
        if (ForbiddenWhitespace.Any(value.Contains)) return false;

        // 4. Basic character sanity (no spaces already ensured). Could extend with allowed charset.
        return true;
    }

    public override string ToString() => Value;
}
