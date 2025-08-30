using System;

namespace AcmeStore.Domain.Users.ValueObjects;

/// <summary>
/// PersonName value object with optional title and required first/last names.
/// </summary>
public sealed record class PersonName
{
	public string? Title { get; }
	public string FirstName { get; }
	public string LastName { get; }

	private PersonName(string? title, string firstName, string lastName)
	{
		Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
		FirstName = firstName;
		LastName = lastName;
	}

	public static PersonName Create(string? title, string? firstName, string? lastName)
	{
		if (string.IsNullOrWhiteSpace(firstName))
		{
			throw new ArgumentException("First name is required.", nameof(firstName));
		}
		if (string.IsNullOrWhiteSpace(lastName))
		{
			throw new ArgumentException("Last name is required.", nameof(lastName));
		}

		var fn = firstName.Trim();
		var ln = lastName.Trim();

		if (fn.Length > 100 || ln.Length > 100)
		{
			throw new ArgumentException("Name components are too long.");
		}

		return new PersonName(title, fn, ln);
	}

	public override string ToString()
	{
		return Title is null ? $"{FirstName} {LastName}" : $"{Title} {FirstName} {LastName}";
	}
}


