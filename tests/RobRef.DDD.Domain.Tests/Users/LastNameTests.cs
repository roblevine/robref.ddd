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

    [Fact]
    public void CompareTo_SameLastNameValues_ReturnsZero()
    {
        // Arrange
        var name1 = new LastName("Smith");
        var name2 = new LastName("Smith");
        
        // Act & Assert
        Assert.Equal(0, name1.CompareTo(name2));
        Assert.Equal(0, name2.CompareTo(name1));
    }

    [Fact]
    public void CompareTo_DifferentLastNameValues_ReturnsCorrectOrder()
    {
        // Arrange
        var nameA = new LastName("Adams");
        var nameB = new LastName("Brown");
        
        // Act & Assert
        Assert.True(nameA.CompareTo(nameB) < 0); // Adams comes before Brown
        Assert.True(nameB.CompareTo(nameA) > 0); // Brown comes after Adams
    }

    [Fact]
    public void CompareTo_CaseInsensitive_ReturnsZero()
    {
        // Arrange
        var name1 = new LastName("SMITH");
        var name2 = new LastName("smith");
        
        // Act & Assert
        Assert.Equal(0, name1.CompareTo(name2));
        Assert.Equal(0, name2.CompareTo(name1));
    }

    [Fact]
    public void CompareTo_NullLastName_ReturnsOne()
    {
        // Arrange
        var name = new LastName("Smith");
        
        // Act & Assert
        Assert.Equal(1, name.CompareTo(null));
    }

    [Fact]
    public void CompareTo_SupportsListSorting()
    {
        // Arrange
        var names = new List<LastName>
        {
            new("Wilson"),
            new("Adams"),
            new("Brown")
        };
        
        // Act
        names.Sort();
        
        // Assert
        Assert.Equal("Adams", names[0].Value);
        Assert.Equal("Brown", names[1].Value);
        Assert.Equal("Wilson", names[2].Value);
    }
}