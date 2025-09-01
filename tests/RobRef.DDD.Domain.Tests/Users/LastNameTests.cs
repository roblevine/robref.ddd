using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Domain.Tests.Users;

public class LastNameTests
{
    [Theory]
    [InlineData("Smith")]
    [InlineData("Johnson")]
    [InlineData("Van Der Berg")]
    [InlineData("O'Connor")]
    [InlineData("李")]
    [InlineData("García")]
    public void Constructor_ValidLastName_CreatesInstance(string validName)
    {
        // Act
        var lastName = new LastName(validName);
        
        // Assert
        Assert.Equal(validName, lastName.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Constructor_NullOrWhitespace_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new LastName(invalidName));
    }

    [Theory]
    [InlineData("This surname is incredibly long and unreasonable for any practical use in a system")]
    public void Constructor_TooLong_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new LastName(invalidName));
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var lastName1 = new LastName("Smith");
        var lastName2 = new LastName("Smith");
        
        // Act & Assert
        Assert.Equal(lastName1, lastName2);
        Assert.True(lastName1 == lastName2);
        Assert.False(lastName1 != lastName2);
        Assert.Equal(lastName1.GetHashCode(), lastName2.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Arrange
        var lastName1 = new LastName("Smith");
        var lastName2 = new LastName("Johnson");
        
        // Act & Assert
        Assert.NotEqual(lastName1, lastName2);
        Assert.False(lastName1 == lastName2);
        Assert.True(lastName1 != lastName2);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var name = "Smith";
        var lastName = new LastName(name);
        
        // Act
        var result = lastName.ToString();
        
        // Assert
        Assert.Equal(name, result);
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesLastName()
    {
        // Arrange
        string name = "Smith";
        
        // Act
        LastName lastName = name;
        
        // Assert
        Assert.Equal(name, lastName.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        // Arrange
        var lastName = new LastName("Smith");
        
        // Act
        string result = lastName;
        
        // Assert
        Assert.Equal("Smith", result);
    }
}