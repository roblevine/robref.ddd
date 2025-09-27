using RobRef.DDD.Products.Domain;
using Xunit;

namespace RobRef.DDD.Products.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateProduct()
    {
        var productId = ProductId.Create();
        var productName = new ProductName("Test Product");
        var productDescription = new ProductDescription("Test product description");
        
        var product = Product.Create(productId, productName, productDescription);
        
        Assert.Equal(productId, product.Id);
        Assert.Equal(productName, product.Name);
        Assert.Equal(productDescription, product.Description);
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowArgumentNullException()
    {
        var productId = ProductId.Create();
        var productDescription = new ProductDescription("Test product description");
        
        var exception = Assert.Throws<ArgumentNullException>(() => 
            Product.Create(productId, null!, productDescription));
        Assert.Equal("productName", exception.ParamName);
    }

    [Fact]
    public void Create_WithNullDescription_ShouldThrowArgumentNullException()
    {
        var productId = ProductId.Create();
        var productName = new ProductName("Test Product");
        
        var exception = Assert.Throws<ArgumentNullException>(() => 
            Product.Create(productId, productName, null!));
        Assert.Equal("productDescription", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldCreateProduct()
    {
        var productId = ProductId.Create();
        var productName = new ProductName("Test Product");
        var productDescription = new ProductDescription("");
        
        var product = Product.Create(productId, productName, productDescription);
        
        Assert.Equal(productId, product.Id);
        Assert.Equal(productName, product.Name);
        Assert.Equal(productDescription, product.Description);
        Assert.Equal(string.Empty, product.Description.Value);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateProductName()
    {
        var product = CreateTestProduct();
        var newName = new ProductName("Updated Product Name");
        
        product.UpdateName(newName);
        
        Assert.Equal(newName, product.Name);
    }

    [Fact]
    public void UpdateName_WithNullName_ShouldThrowArgumentNullException()
    {
        var product = CreateTestProduct();
        
        var exception = Assert.Throws<ArgumentNullException>(() => product.UpdateName(null!));
        Assert.Equal("productName", exception.ParamName);
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldUpdateProductDescription()
    {
        var product = CreateTestProduct();
        var newDescription = new ProductDescription("Updated product description with more details");
        
        product.UpdateDescription(newDescription);
        
        Assert.Equal(newDescription, product.Description);
    }

    [Fact]
    public void UpdateDescription_WithNullDescription_ShouldThrowArgumentNullException()
    {
        var product = CreateTestProduct();
        
        var exception = Assert.Throws<ArgumentNullException>(() => product.UpdateDescription(null!));
        Assert.Equal("productDescription", exception.ParamName);
    }

    [Fact]
    public void UpdateDescription_WithEmptyDescription_ShouldUpdateProductDescription()
    {
        var product = CreateTestProduct();
        var emptyDescription = new ProductDescription("");
        
        product.UpdateDescription(emptyDescription);
        
        Assert.Equal(emptyDescription, product.Description);
        Assert.Equal(string.Empty, product.Description.Value);
    }

    [Fact]
    public void Equality_WithSameId_ShouldBeEqual()
    {
        var productId = ProductId.Create();
        var productName1 = new ProductName("Product A");
        var productName2 = new ProductName("Product B");
        var productDescription1 = new ProductDescription("Description A");
        var productDescription2 = new ProductDescription("Description B");
        
        var product1 = Product.Create(productId, productName1, productDescription1);
        var product2 = Product.Create(productId, productName2, productDescription2);
        
        Assert.True(product1.Equals(product2));
        Assert.True(product1 == product2);
        Assert.False(product1 != product2);
        Assert.Equal(product1.GetHashCode(), product2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentId_ShouldNotBeEqual()
    {
        var productName = new ProductName("Same Product");
        var productDescription = new ProductDescription("Same description");
        
        var product1 = Product.Create(ProductId.Create(), productName, productDescription);
        var product2 = Product.Create(ProductId.Create(), productName, productDescription);
        
        Assert.False(product1.Equals(product2));
        Assert.False(product1 == product2);
        Assert.True(product1 != product2);
    }

    [Fact]
    public void ToString_ShouldReturnMeaningfulRepresentation()
    {
        var productId = ProductId.Create();
        var productName = new ProductName("Test Product");
        var productDescription = new ProductDescription("Test description");
        
        var product = Product.Create(productId, productName, productDescription);
        var result = product.ToString();
        
        Assert.Contains("Test Product", result);
        Assert.Contains(productId.ToString(), result);
    }

    [Theory]
    [InlineData("Simple Product", "Simple description")]
    [InlineData("Product with Émojis 🔥", "Description with unicode ñáéíóú")]
    [InlineData("A", "")]
    public void Create_WithVariousValidInputs_ShouldCreateProduct(string name, string description)
    {
        var productId = ProductId.Create();
        var productName = new ProductName(name);
        var productDescription = new ProductDescription(description);
        
        var product = Product.Create(productId, productName, productDescription);
        
        Assert.Equal(productId, product.Id);
        Assert.Equal(productName, product.Name);
        Assert.Equal(productDescription, product.Description);
    }

    private static Product CreateTestProduct()
    {
        return Product.Create(
            ProductId.Create(),
            new ProductName("Test Product"),
            new ProductDescription("Test product description"));
    }
}