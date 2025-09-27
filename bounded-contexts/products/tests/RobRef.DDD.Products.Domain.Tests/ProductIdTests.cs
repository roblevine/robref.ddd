using RobRef.DDD.Products.Domain;

namespace RobRef.DDD.Products.Domain.Tests;

public class ProductIdTests
{
    [Fact]
    public void Create_ShouldGenerateValidUlid()
    {
        // Act
        var productId = ProductId.Create();

        // Assert
        Assert.NotEqual(ProductId.Empty, productId);
        Assert.True(Ulid.TryParse(productId.ToString(), out _));
    }

    [Fact]
    public void Create_ShouldGenerateUniqueValues()
    {
        // Act
        var id1 = ProductId.Create();
        var id2 = ProductId.Create();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id1.Value, id2.Value);
    }

    [Fact]
    public void Empty_ShouldReturnEmptyUlid()
    {
        // Act
        var empty = ProductId.Empty;

        // Assert
        Assert.Equal(Ulid.Empty, empty.Value);
        Assert.Equal("00000000000000000000000000", empty.ToString());
    }

    [Fact]
    public void ToString_ShouldReturnUlidString()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var productId = new ProductId(ulid);

        // Act
        var result = productId.ToString();

        // Assert
        Assert.Equal(ulid.ToString(), result);
    }

    [Fact]
    public void Parse_WithValidUlidString_ShouldReturnProductId()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var ulidString = ulid.ToString();

        // Act
        var productId = ProductId.Parse(ulidString);

        // Assert
        Assert.Equal(ulid, productId.Value);
    }

    [Fact]
    public void Parse_WithInvalidString_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => ProductId.Parse("invalid"));
    }

    [Fact]
    public void TryParse_WithValidUlidString_ShouldReturnTrueAndProductId()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var ulidString = ulid.ToString();

        // Act
        var success = ProductId.TryParse(ulidString, out var productId);

        // Assert
        Assert.True(success);
        Assert.Equal(ulid, productId.Value);
    }

    [Fact]
    public void TryParse_WithInvalidString_ShouldReturnFalseAndEmpty()
    {
        // Act
        var success = ProductId.TryParse("invalid", out var productId);

        // Assert
        Assert.False(success);
        Assert.Equal(ProductId.Empty, productId);
    }

    [Fact]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var id1 = new ProductId(ulid);
        var id2 = new ProductId(ulid);

        // Act & Assert
        Assert.Equal(id1, id2);
        Assert.True(id1 == id2);
        Assert.False(id1 != id2);
        Assert.True(id1.Equals(id2));
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var id1 = ProductId.Create();
        var id2 = ProductId.Create();

        // Act & Assert
        Assert.NotEqual(id1, id2);
        Assert.False(id1 == id2);
        Assert.True(id1 != id2);
        Assert.False(id1.Equals(id2));
    }

    [Fact]
    public void CompareTo_ShouldOrderByUlidValue()
    {
        // Arrange
        var earlier = Ulid.Parse("01ARZ3NDEKTSV4RRFFQ69G5FAV");
        var later = Ulid.Parse("01ARZ3NDEKTSV4RRFFQ69G5FBV");
        var id1 = new ProductId(earlier);
        var id2 = new ProductId(later);

        // Act & Assert
        Assert.True(id1.CompareTo(id2) < 0);
        Assert.True(id2.CompareTo(id1) > 0);
        Assert.Equal(0, id1.CompareTo(id1));
    }

    [Fact]
    public void ImplicitConversion_FromProductIdToString_ShouldWork()
    {
        // Arrange
        var productId = ProductId.Create();

        // Act
        string str = productId;

        // Assert
        Assert.Equal(productId.ToString(), str);
    }

    [Fact]
    public void ImplicitConversion_FromStringToProductId_ShouldWork()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var ulidString = ulid.ToString();

        // Act
        ProductId productId = ulidString;

        // Assert
        Assert.Equal(ulid, productId.Value);
    }

    [Fact]
    public void ImplicitConversion_FromInvalidString_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { ProductId id = "invalid"; });
    }
}