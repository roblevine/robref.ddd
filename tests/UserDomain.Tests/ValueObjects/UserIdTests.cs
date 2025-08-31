using System;
using Xunit;

namespace UserDomain.Tests.ValueObjects;

public class UserIdTests
{
    [Fact]
    public void UserId_GeneratesNewGuid()
    {
        var id = UserId.New(); // Should exist after implementation
        var id2 = UserId.New();

        Assert.NotEqual(id, id2); // Value inequality expected (almost always)
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-guid")] 
    [InlineData("12345678-1234-1234-1234-123412341234XXX")] // too long
    public void UserId_RejectsInvalidGuidString(string raw)
    {
        Assert.False(UserId.TryParse(raw, out _));
    }
}
