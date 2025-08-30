using System;

namespace AcmeStore.Domain.Tests.Users.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_NormalizesAndEqualsCaseInsensitively()
    {
        var a = AcmeStore.Domain.Users.ValueObjects.Email.Create("  Alice.Example@Example.COM  ");
        var b = AcmeStore.Domain.Users.ValueObjects.Email.Create("alice.example@example.com");

        Assert.Equal(b, a);
        Assert.Equal("alice.example@example.com", a.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("missing-at.example.com")]
    [InlineData("missing-domain@")]
    public void Create_Throws_OnInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => AcmeStore.Domain.Users.ValueObjects.Email.Create(input));
    }
}


