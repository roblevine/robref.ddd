using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Domain.Tests.Users;

public class TitleTests
{
    [Theory]
    [InlineData("Mr")]
    [InlineData("Ms")]
    [InlineData("Mrs")]
    [InlineData("Dr")]
    [InlineData("Prof")]
    [InlineData("Sir")]
    [InlineData("Dame")]
    public void Constructor_ValidTitle_CreatesInstance(string validTitle)
    {
        // Act
        var title = new Title(validTitle);
        
        // Assert
        Assert.Equal(validTitle, title.Value);
    }

    [Fact]
    public void Constructor_Null_CreatesInstanceWithNull()
    {
        // Act
        var title = new Title(null);
        
        // Assert
        Assert.Null(title.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Constructor_EmptyOrWhitespace_ThrowsArgumentException(string invalidTitle)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Title(invalidTitle));
    }

    [Theory]
    [InlineData("")]
    [InlineData("This title is way too long to be reasonable")]
    public void Constructor_TooShortOrTooLong_ThrowsArgumentException(string invalidTitle)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Title(invalidTitle));
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var title1 = new Title("Dr");
        var title2 = new Title("Dr");
        
        // Act & Assert
        Assert.Equal(title1, title2);
        Assert.True(title1 == title2);
        Assert.False(title1 != title2);
        Assert.Equal(title1.GetHashCode(), title2.GetHashCode());
    }

    [Fact]
    public void Equality_BothNull_AreEqual()
    {
        // Arrange
        var title1 = new Title(null);
        var title2 = new Title(null);
        
        // Act & Assert
        Assert.Equal(title1, title2);
        Assert.True(title1 == title2);
        Assert.False(title1 != title2);
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Arrange
        var title1 = new Title("Dr");
        var title2 = new Title("Mr");
        
        // Act & Assert
        Assert.NotEqual(title1, title2);
        Assert.False(title1 == title2);
        Assert.True(title1 != title2);
    }

    [Fact]
    public void ToString_WithValue_ReturnsValue()
    {
        // Arrange
        var titleValue = "Dr";
        var title = new Title(titleValue);
        
        // Act
        var result = title.ToString();
        
        // Assert
        Assert.Equal(titleValue, result);
    }

    [Fact]
    public void ToString_WithNull_ReturnsEmptyString()
    {
        // Arrange
        var title = new Title(null);
        
        // Act
        var result = title.ToString();
        
        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesTitle()
    {
        // Arrange
        string titleValue = "Dr";
        
        // Act
        Title title = titleValue;
        
        // Assert
        Assert.Equal(titleValue, title.Value);
    }

    [Fact]
    public void ImplicitConversion_FromNull_CreatesTitle()
    {
        // Arrange
        string? titleValue = null;
        
        // Act
        Title title = titleValue;
        
        // Assert
        Assert.Null(title.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        // Arrange
        var title = new Title("Dr");
        
        // Act
        string? result = title;
        
        // Assert
        Assert.Equal("Dr", result);
    }

    [Fact]
    public void ImplicitConversion_NullToString_ReturnsNull()
    {
        // Arrange
        var title = new Title(null);
        
        // Act
        string? result = title;
        
        // Assert
        Assert.Null(result);
    }
}