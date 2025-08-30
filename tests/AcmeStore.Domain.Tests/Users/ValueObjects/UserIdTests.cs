using System;

namespace AcmeStore.Domain.Tests.Users.ValueObjects;

public class UserIdTests
{
    [Fact]
    public void New_GeneratesNonEmptyGuidBackedValue()
    {
        var userId = AcmeStore.Domain.Users.ValueObjects.UserId.New();
        Assert.NotEqual(Guid.Empty, userId.Value);
    }

    [Fact]
    public void From_Throws_WhenGuidIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => AcmeStore.Domain.Users.ValueObjects.UserId.From(Guid.Empty));
    }

    [Fact]
    public void Parse_ReturnsExpected_WhenValidGuidString()
    {
        var guid = Guid.NewGuid();
        var userId = AcmeStore.Domain.Users.ValueObjects.UserId.Parse(guid.ToString());
        Assert.Equal(guid, userId.Value);
    }

    [Fact]
    public void TryParse_ReturnsFalse_WhenInvalidGuidString()
    {
        var success = AcmeStore.Domain.Users.ValueObjects.UserId.TryParse("not-a-guid", out var _);
        Assert.False(success);
    }
}


