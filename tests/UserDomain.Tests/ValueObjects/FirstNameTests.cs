using System;
using Xunit;

namespace UserDomain.Tests.ValueObjects;

public class FirstNameTests
{
    [Theory]
    [InlineData("Ada")]
    [InlineData("María")] // accented
    [InlineData("Jean-Luc")]
    public void FirstName_AcceptsValid(string raw)
    {
        var f = FirstName.Create(raw);
        Assert.NotNull(f);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Al3x")]
    [InlineData("John*")]
    public void FirstName_RejectsInvalid(string raw)
    {
        Assert.False(FirstName.TryCreate(raw, out _));
    }

    [Fact]
    public void FirstName_Equality_IgnoresCase()
    {
        var a = FirstName.Create("Grace");
        var b = FirstName.Create("grace");
        Assert.Equal(a, b);
    }
}