using System;
using Xunit;

namespace UserDomain.Tests.ValueObjects;

public class TitleTests
{
    [Theory]
    [InlineData("Dr")]
    [InlineData("Prof.")]
    [InlineData("  Ms  ")] // trimming + collapse
    public void Title_AcceptsValidSamples(string raw)
    {
        var t = Title.Create(raw.Trim());
        Assert.NotNull(t);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("Dr#")]
    [InlineData("")] 
    [InlineData(" ")] 
    public void Title_RejectsInvalid(string raw)
    {
        Assert.False(Title.TryCreate(raw, out _));
    }

    [Fact]
    public void Title_Equality_IgnoresCase()
    {
        var a = Title.Create("Dr");
        var b = Title.Create("dr");
        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}