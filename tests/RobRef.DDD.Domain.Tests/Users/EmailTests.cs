using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Domain.Tests.Users;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.email@domain.co.uk")]
    [InlineData("user+tag@example.com")]
    [InlineData("123@example.com")]
    public void Constructor_ValidEmail_CreatesInstance(string validEmail)
    {
        // Act
        var email = new Email(validEmail);
        
        // Assert
        Assert.Equal(validEmail, email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user.example.com")]
    public void Constructor_InvalidEmail_ThrowsArgumentException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
    }

    [Fact]
    public void Equality_SameEmailValues_AreEqual()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");
        
        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
        Assert.False(email1 != email2);
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentEmailValues_AreNotEqual()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");
        
        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.False(email1 == email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void ToString_ReturnsEmailValue()
    {
        // Arrange
        var emailValue = "test@example.com";
        var email = new Email(emailValue);
        
        // Act
        var result = email.ToString();
        
        // Assert
        Assert.Equal(emailValue, result);
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesEmail()
    {
        // Arrange
        string emailValue = "test@example.com";
        
        // Act
        Email email = emailValue;
        
        // Assert
        Assert.Equal(emailValue, email.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        // Arrange
        var email = new Email("test@example.com");
        
        // Act
        string result = email;
        
        // Assert
        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void CompareTo_SameEmailValues_ReturnsZero()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");
        
        // Act & Assert
        Assert.Equal(0, email1.CompareTo(email2));
        Assert.Equal(0, email2.CompareTo(email1));
    }

    [Fact]
    public void CompareTo_DifferentEmailValues_ReturnsCorrectOrder()
    {
        // Arrange
        var emailA = new Email("a@example.com");
        var emailB = new Email("b@example.com");
        
        // Act & Assert
        Assert.True(emailA.CompareTo(emailB) < 0); // a comes before b
        Assert.True(emailB.CompareTo(emailA) > 0); // b comes after a
    }

    [Fact]
    public void CompareTo_CaseInsensitive_ReturnsZero()
    {
        // Arrange
        var email1 = new Email("Test@Example.com");
        var email2 = new Email("test@example.com");
        
        // Act & Assert
        Assert.Equal(0, email1.CompareTo(email2));
        Assert.Equal(0, email2.CompareTo(email1));
    }

    [Fact]
    public void CompareTo_NullEmail_ReturnsOne()
    {
        // Arrange
        var email = new Email("test@example.com");
        
        // Act & Assert
        Assert.Equal(1, email.CompareTo(null));
    }

    [Fact]
    public void CompareTo_SupportsListSorting()
    {
        // Arrange
        var emails = new List<Email>
        {
            new("zebra@example.com"),
            new("alpha@example.com"),
            new("beta@example.com")
        };
        
        // Act
        emails.Sort();
        
        // Assert
        Assert.Equal("alpha@example.com", emails[0].Value);
        Assert.Equal("beta@example.com", emails[1].Value);
        Assert.Equal("zebra@example.com", emails[2].Value);
    }
}