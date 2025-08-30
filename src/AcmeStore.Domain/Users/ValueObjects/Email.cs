using System;
using System.Text.RegularExpressions;

namespace AcmeStore.Domain.Users.ValueObjects;

/// <summary>
/// Email value object with normalization and validation.
/// </summary>
public sealed record class Email
{
	private static readonly Regex EmailRegex = new(
		pattern: @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
		options: RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
		matchTimeout: TimeSpan.FromMilliseconds(250));

	public string Value { get; }

	private Email(string value)
	{
		Value = value;
	}

	public static Email Create(string? input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			throw new ArgumentException("Email cannot be empty.", nameof(input));
		}

		var normalized = Normalize(input);

		if (normalized.Length > 254)
		{
			throw new ArgumentException("Email is too long.", nameof(input));
		}

		if (!EmailRegex.IsMatch(normalized))
		{
			throw new ArgumentException("Invalid email format.", nameof(input));
		}

		return new Email(normalized);
	}

	private static string Normalize(string input)
	{
		var trimmed = input.Trim();
		return trimmed.ToLowerInvariant();
	}

	public override string ToString() => Value;
}


