using System;
using Xunit;

namespace UserDomain.Tests.ValueObjects;

public class EmailAddressTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("USER@EXAMPLE.COM")]
    [InlineData(" user.name+tag@sub.example.co.uk ")]
    public void EmailAddress_AcceptsValidSamples(string raw)
    {
        var email = EmailAddress.Create(raw);
        Assert.NotNull(email);
    }

    [Fact]
    public void EmailAddress_NormalizesToLowerCase()
    {
        var email = EmailAddress.Create("MiXeD.Case+Tag@Example.Com");
        Assert.Equal("mixed.case+tag@example.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("no-at-symbol")]
    [InlineData("two@@example.com")]
    [InlineData("@nouser.com")]
    [InlineData("user@")]
    [InlineData("user@example")]
    [InlineData("user@example.")]
    [InlineData("user@.example.com")]
    [InlineData("user example@domain.com")]
    public void EmailAddress_RejectsInvalidFormats(string raw)
    {
        Assert.Throws<ArgumentException>(() => EmailAddress.Create(raw));
        Assert.False(EmailAddress.TryParse(raw, out _));
    }
}
