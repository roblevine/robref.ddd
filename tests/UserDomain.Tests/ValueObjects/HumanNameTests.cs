using Xunit;

namespace UserDomain.Tests.ValueObjects;

public class HumanNameTests
{
    [Fact]
    public void HumanName_AcceptsValidBasicName_NoTitle()
    {
        var created = HumanName.Create(null, "Ada", "Lovelace");
        Assert.Equal("Ada", created.FirstName.Value);
        Assert.Equal("Lovelace", created.LastName.Value);
        Assert.Null(created.Title);
        Assert.Equal("Ada Lovelace", created.ToString());
    }

    [Fact]
    public void HumanName_AcceptsTitle()
    {
        var created = HumanName.Create("Dr", "Grace", "Hopper");
        Assert.Equal("Dr", created.Title!.Value);
        Assert.Equal("Dr Grace Hopper", created.ToString());
    }

    [Fact]
    public void HumanName_AcceptsHyphenApostropheAccents()
    {
        var created = HumanName.Create("Ms", "María-José", "O’Neill");
        Assert.Equal("María-José", created.FirstName.Value);
        Assert.Equal("O’Neill", created.LastName.Value);
    }

    [Theory]
    [InlineData("Al3x", "Smith")] // digit
    [InlineData("Alex", "Sm!th")] // symbol
    [InlineData("", "Smith")] // empty first
    [InlineData("Alex", " ")] // whitespace last
    public void HumanName_RejectsInvalidCharacters(string first, string last)
    {
    var firstOk = FirstName.TryCreate(first, out _);
    var lastOk = LastName.TryCreate(last, out _);
    Assert.False(firstOk && lastOk);
    }

    [Fact]
    public void HumanName_TrimsAndCollapsesWhitespace()
    {
        var ok = HumanName.Create("  Dr  ", "  Mary   Ann  ", "  Van   Buren ");
        Assert.Equal("Dr", ok.Title!.Value);
        Assert.Equal("Mary Ann", ok.FirstName.Value); // collapsed internal spaces
        Assert.Equal("Van Buren", ok.LastName.Value);
    }

    [Fact]
    public void HumanName_Equality_IgnoresCase()
    {
        var a = HumanName.Create("Dr", "Grace", "Hopper");
        var b = HumanName.Create("dr", "grace", "HOPPER");
    Assert.True(a.Equals(b));
    Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void HumanName_TryCreate_Fails_InvalidTitle()
    {
        Assert.False(Title.TryCreate("Dr3", out _));
    }
}