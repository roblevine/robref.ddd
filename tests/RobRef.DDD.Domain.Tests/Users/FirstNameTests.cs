using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Domain.Tests.Users;

public class FirstNameTests
{
    [Theory]
    [InlineData("John")]
    [InlineData("Mary")]
    [InlineData("Jean-Pierre")]
    [InlineData("O'Connor")]
    [InlineData("李")]
    [InlineData("José")]
    public void Constructor_ValidFirstName_CreatesInstance(string validName)
    {
        // Act
        var firstName = new FirstName(validName);
        
        // Assert
        Assert.Equal(validName, firstName.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Constructor_NullOrWhitespace_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FirstName(invalidName));
    }

    [Theory]
    [InlineData("This name is way too long to be a reasonable first name for any practical purpose")]
    public void Constructor_TooLong_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FirstName(invalidName));
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var firstName1 = new FirstName("John");
        var firstName2 = new FirstName("John");
        
        // Act & Assert
        Assert.Equal(firstName1, firstName2);
        Assert.True(firstName1 == firstName2);
        Assert.False(firstName1 != firstName2);
        Assert.Equal(firstName1.GetHashCode(), firstName2.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Arrange
        var firstName1 = new FirstName("John");
        var firstName2 = new FirstName("Jane");
        
        // Act & Assert
        Assert.NotEqual(firstName1, firstName2);
        Assert.False(firstName1 == firstName2);
        Assert.True(firstName1 != firstName2);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var name = "John";
        var firstName = new FirstName(name);
        
        // Act
        var result = firstName.ToString();
        
        // Assert
        Assert.Equal(name, result);
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesFirstName()
    {
        // Arrange
        string name = "John";
        
        // Act
        FirstName firstName = name;
        
        // Assert
        Assert.Equal(name, firstName.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        // Arrange
        var firstName = new FirstName("John");
        
        // Act
        string result = firstName;
        
        // Assert
        Assert.Equal("John", result);
    }
}