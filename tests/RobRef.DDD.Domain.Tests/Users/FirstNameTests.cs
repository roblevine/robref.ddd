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

    [Fact]
    public void CompareTo_SameFirstNameValues_ReturnsZero()
    {
        // Arrange
        var name1 = new FirstName("John");
        var name2 = new FirstName("John");
        
        // Act & Assert
        Assert.Equal(0, name1.CompareTo(name2));
        Assert.Equal(0, name2.CompareTo(name1));
    }

    [Fact]
    public void CompareTo_DifferentFirstNameValues_ReturnsCorrectOrder()
    {
        // Arrange
        var nameA = new FirstName("Alice");
        var nameB = new FirstName("Bob");
        
        // Act & Assert
        Assert.True(nameA.CompareTo(nameB) < 0); // Alice comes before Bob
        Assert.True(nameB.CompareTo(nameA) > 0); // Bob comes after Alice
    }

    [Fact]
    public void CompareTo_CaseInsensitive_ReturnsZero()
    {
        // Arrange
        var name1 = new FirstName("JOHN");
        var name2 = new FirstName("john");
        
        // Act & Assert
        Assert.Equal(0, name1.CompareTo(name2));
        Assert.Equal(0, name2.CompareTo(name1));
    }

    [Fact]
    public void CompareTo_NullFirstName_ReturnsOne()
    {
        // Arrange
        var name = new FirstName("John");
        
        // Act & Assert
        Assert.Equal(1, name.CompareTo(null));
    }

    [Fact]
    public void CompareTo_SupportsListSorting()
    {
        // Arrange
        var names = new List<FirstName>
        {
            new("Zoe"),
            new("Alice"),
            new("Bob")
        };
        
        // Act
        names.Sort();
        
        // Assert
        Assert.Equal("Alice", names[0].Value);
        Assert.Equal("Bob", names[1].Value);
        Assert.Equal("Zoe", names[2].Value);
    }
}