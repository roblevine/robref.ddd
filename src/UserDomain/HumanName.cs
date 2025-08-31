using System;
using System.Globalization;
using System.Text;

namespace UserDomain;

/// <summary>
/// Human name value object composed of optional Title and required First/Last names.
/// Equality is case-insensitive across all parts; original casing preserved for display.
/// Declared as sealed record class to leverage with-expression & deconstruction while overriding equality semantics.
/// </summary>
public sealed record class HumanName : IEquatable<HumanName>
{
    public Title? Title { get; }
    public FirstName FirstName { get; }
    public LastName LastName { get; }

    private HumanName(Title? title, FirstName first, LastName last)
    {
        Title = title;
        FirstName = first;
        LastName = last;
    }

    public static HumanName Create(Title? title, FirstName first, LastName last) => new(title, first, last);

    public static HumanName Create(string? titleRaw, string firstRaw, string lastRaw)
    {
        var title = Title.TryCreate(titleRaw, out var t) ? t : null;
        return new HumanName(title, FirstName.Create(firstRaw), LastName.Create(lastRaw));
    }

    public override string ToString() => Title is null
        ? $"{FirstName} {LastName}"
        : $"{Title} {FirstName} {LastName}";

    public bool Equals(HumanName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        var titlesEqual = (Title is null && other.Title is null) || (Title?.Equals(other.Title) ?? false);
        return titlesEqual && FirstName.Equals(other.FirstName) && LastName.Equals(other.LastName);
    }

    public override int GetHashCode()
        => HashCode.Combine(Title?.GetHashCode() ?? 0, FirstName.GetHashCode(), LastName.GetHashCode());
}

// Shared extensions relocated to individual VOs.