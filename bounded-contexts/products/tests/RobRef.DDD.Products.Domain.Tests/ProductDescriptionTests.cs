using RobRef.DDD.Products.Domain;
using Xunit;

namespace RobRef.DDD.Products.Domain.Tests;

public class ProductDescriptionTests
{
    [Fact]
    public void MaxLength_ShouldBe8095()
    {
        Assert.Equal(8095, ProductDescription.MaxLength);
    }

    [Fact]
    public void Constructor_WithValidValue_ShouldCreateProductDescription()
    {
        var description = new ProductDescription("This is a test product description.");
        
        Assert.Equal("This is a test product description.", description.Value);
    }

    [Fact]
    public void Constructor_WithLeadingAndTrailingWhitespace_ShouldTrimValue()
    {
        var description = new ProductDescription("   This is a trimmed description.   ");
        
        Assert.Equal("This is a trimmed description.", description.Value);
    }

    [Fact]
    public void Constructor_WithNullValue_ShouldCreateEmptyProductDescription()
    {
        var description = new ProductDescription(null);
        
        Assert.Equal(string.Empty, description.Value);
    }

    [Fact]
    public void Constructor_WithEmptyValue_ShouldCreateEmptyProductDescription()
    {
        var description = new ProductDescription(string.Empty);
        
        Assert.Equal(string.Empty, description.Value);
    }

    [Fact]
    public void Constructor_WithWhitespaceValue_ShouldCreateEmptyProductDescription()
    {
        var description = new ProductDescription("   ");
        
        Assert.Equal(string.Empty, description.Value);
    }

    [Fact]
    public void Constructor_WithMaxLengthValue_ShouldCreateProductDescription()
    {
        var value = new string('A', ProductDescription.MaxLength);
        var description = new ProductDescription(value);
        
        Assert.Equal(value, description.Value);
    }

    [Fact]
    public void Constructor_WithTooLongValue_ShouldThrowArgumentException()
    {
        var value = new string('A', ProductDescription.MaxLength + 1);
        
        var exception = Assert.Throws<ArgumentException>(() => new ProductDescription(value));
        Assert.Contains("cannot exceed", exception.Message);
        Assert.Contains("8095", exception.Message);
    }

    [Theory]
    [InlineData("Simple product description")]
    [InlineData("Product description with 123 numbers")]
    [InlineData("Product-description-with-dashes")]
    [InlineData("Product_description_with_underscores")]
    [InlineData("Product description with special characters: @#$%^&*()")]
    [InlineData("Multi-line\nproduct\ndescription")]
    [InlineData("Product description with 🔥 emoji and unicode")]
    [InlineData("")]
    public void Constructor_WithValidVariations_ShouldCreateProductDescription(string value)
    {
        var description = new ProductDescription(value);
        var expectedValue = value?.Trim() ?? string.Empty;
        
        Assert.Equal(expectedValue, description.Value);
    }

    [Fact]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        var description1 = new ProductDescription("Same description");
        var description2 = new ProductDescription("Same description");
        
        Assert.True(description1.Equals(description2));
        Assert.True(description1 == description2);
        Assert.False(description1 != description2);
        Assert.Equal(description1.GetHashCode(), description2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        var description1 = new ProductDescription("First description");
        var description2 = new ProductDescription("Second description");
        
        Assert.False(description1.Equals(description2));
        Assert.False(description1 == description2);
        Assert.True(description1 != description2);
    }

    [Fact]
    public void Equality_WithNullValues_ShouldBeEqual()
    {
        var description1 = new ProductDescription(null);
        var description2 = new ProductDescription(string.Empty);
        var description3 = new ProductDescription("   ");
        
        Assert.True(description1.Equals(description2));
        Assert.True(description1.Equals(description3));
        Assert.True(description2.Equals(description3));
    }

    [Fact]
    public void CompareTo_ShouldOrderByValue()
    {
        var descriptionA = new ProductDescription("A description");
        var descriptionB = new ProductDescription("B description");
        var descriptionZ = new ProductDescription("Z description");
        var emptyDescription = new ProductDescription("");
        
        Assert.True(emptyDescription.CompareTo(descriptionA) < 0);
        Assert.True(descriptionA.CompareTo(descriptionB) < 0);
        Assert.True(descriptionB.CompareTo(descriptionZ) < 0);
        Assert.True(descriptionZ.CompareTo(descriptionA) > 0);
        Assert.Equal(0, descriptionA.CompareTo(descriptionA));
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        var description = new ProductDescription("Test description");
        
        Assert.Equal("Test description", description.ToString());
    }

    [Fact]
    public void ImplicitConversion_FromStringToProductDescription_ShouldWork()
    {
        ProductDescription description = "Test description";
        
        Assert.Equal("Test description", description.Value);
    }

    [Fact]
    public void ImplicitConversion_FromNullStringToProductDescription_ShouldWork()
    {
        ProductDescription description = (string?)null;
        
        Assert.Equal(string.Empty, description.Value);
    }

    [Fact]
    public void ImplicitConversion_FromProductDescriptionToString_ShouldWork()
    {
        var description = new ProductDescription("Test description");
        string value = description;
        
        Assert.Equal("Test description", value);
    }

    [Fact]
    public void ImplicitConversion_FromInvalidString_ShouldThrowArgumentException()
    {
        var invalidValue = new string('A', ProductDescription.MaxLength + 1);
        
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            ProductDescription description = invalidValue;
        });
        Assert.Contains("cannot exceed", exception.Message);
    }
}