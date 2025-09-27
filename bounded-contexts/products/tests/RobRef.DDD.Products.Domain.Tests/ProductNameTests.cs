using RobRef.DDD.Products.Domain;

namespace RobRef.DDD.Products.Domain.Tests;

public class ProductNameTests
{
    [Fact]
    public void Constructor_WithValidValue_ShouldCreateProductName()
    {
        // Arrange
        const string value = "Test Product Name";

        // Act
        var productName = new ProductName(value);

        // Assert
        Assert.Equal(value, productName.Value);
    }

    [Fact]
    public void Constructor_WithEmptyValue_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProductName(""));
    }

    [Fact]
    public void Constructor_WithWhitespaceValue_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProductName("   "));
    }

    [Fact]
    public void Constructor_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ProductName(null!));
    }

    [Fact]
    public void Constructor_WithMaxLengthValue_ShouldCreateProductName()
    {
        // Arrange
        var value = new string('a', ProductName.MaxLength);

        // Act
        var productName = new ProductName(value);

        // Assert
        Assert.Equal(value, productName.Value);
    }

    [Fact]
    public void Constructor_WithTooLongValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = new string('a', ProductName.MaxLength + 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProductName(value));
    }

    [Fact]
    public void MaxLength_ShouldBe254()
    {
        // Assert
        Assert.Equal(254, ProductName.MaxLength);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        const string value = "Test Product Name";
        var productName = new ProductName(value);

        // Act
        var result = productName.ToString();

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        const string value = "Test Product Name";
        var name1 = new ProductName(value);
        var name2 = new ProductName(value);

        // Act & Assert
        Assert.Equal(name1, name2);
        Assert.True(name1 == name2);
        Assert.False(name1 != name2);
        Assert.True(name1.Equals(name2));
        Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var name1 = new ProductName("Product One");
        var name2 = new ProductName("Product Two");

        // Act & Assert
        Assert.NotEqual(name1, name2);
        Assert.False(name1 == name2);
        Assert.True(name1 != name2);
        Assert.False(name1.Equals(name2));
    }

    [Fact]
    public void CompareTo_ShouldOrderByValue()
    {
        // Arrange
        var nameA = new ProductName("A Product");
        var nameB = new ProductName("B Product");
        var nameC = new ProductName("C Product");

        // Act & Assert
        Assert.True(nameA.CompareTo(nameB) < 0);
        Assert.True(nameB.CompareTo(nameC) < 0);
        Assert.True(nameC.CompareTo(nameA) > 0);
        Assert.Equal(0, nameA.CompareTo(nameA));
    }

    [Fact]
    public void ImplicitConversion_FromProductNameToString_ShouldWork()
    {
        // Arrange
        const string value = "Test Product Name";
        var productName = new ProductName(value);

        // Act
        string str = productName;

        // Assert
        Assert.Equal(value, str);
    }

    [Fact]
    public void ImplicitConversion_FromStringToProductName_ShouldWork()
    {
        // Arrange
        const string value = "Test Product Name";

        // Act
        ProductName productName = value;

        // Assert
        Assert.Equal(value, productName.Value);
    }

    [Fact]
    public void ImplicitConversion_FromInvalidString_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { ProductName name = ""; });
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Z")]
    [InlineData("Product")]
    [InlineData("Product Name with Spaces")]
    [InlineData("Product-with-Dashes")]
    [InlineData("Product_with_Underscores")]
    [InlineData("Product123")]
    [InlineData("123Product")]
    [InlineData("Product with 🔥 emoji")]
    public void Constructor_WithValidVariations_ShouldCreateProductName(string value)
    {
        // Act
        var productName = new ProductName(value);

        // Assert
        Assert.Equal(value, productName.Value);
    }

    [Fact]
    public void Constructor_WithLeadingAndTrailingWhitespace_ShouldTrimValue()
    {
        // Arrange
        const string value = "  Product Name  ";
        const string expectedValue = "Product Name";

        // Act
        var productName = new ProductName(value);

        // Assert
        Assert.Equal(expectedValue, productName.Value);
    }
}