using System;
using Xunit;

namespace UserDomain.Tests.ValueObjects;

public class LastNameTests
{
    [Theory]
    [InlineData("Lovelace")]
    [InlineData("O’Neill")]
    [InlineData("Van Buren")] // internal space
    public void LastName_AcceptsValid(string raw)
    {
        var l = LastName.Create(raw);
        Assert.NotNull(l);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Sm!th")]
    [InlineData("Smi7h")]
    public void LastName_RejectsInvalid(string raw)
    {
        Assert.False(LastName.TryCreate(raw, out _));
    }

    [Fact]
    public void LastName_Equality_IgnoresCase()
    {
        var a = LastName.Create("Hopper");
        var b = LastName.Create("HOPPER");
        Assert.Equal(a, b);
    }
}