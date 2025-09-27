namespace RobRef.DDD.Products.Domain;

/// <summary>
/// Represents a Product aggregate root in the domain.
/// Products are identified by a unique ProductId and have a required name and optional description.
/// This class implements domain logic and business invariants for products.
/// </summary>
public class Product : IEquatable<Product>
{
    /// <summary>
    /// The unique identifier for the product
    /// </summary>
    public ProductId Id { get; private set; }

    /// <summary>
    /// The product name (required)
    /// </summary>
    public ProductName Name { get; private set; }

    /// <summary>
    /// The product description (optional, can be empty)
    /// </summary>
    public ProductDescription Description { get; private set; }

    /// <summary>
    /// Private constructor to enforce factory method usage
    /// </summary>
    private Product(ProductId id, ProductName name, ProductDescription description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Creates a new Product with the specified parameters.
    /// Factory method that ensures all business invariants are satisfied.
    /// </summary>
    /// <param name="productId">The unique product identifier</param>
    /// <param name="productName">The product name</param>
    /// <param name="productDescription">The product description</param>
    /// <returns>A new Product instance</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
    public static Product Create(ProductId productId, ProductName productName, ProductDescription productDescription)
    {
        ArgumentNullException.ThrowIfNull(productId, nameof(productId));
        ArgumentNullException.ThrowIfNull(productName, nameof(productName));
        ArgumentNullException.ThrowIfNull(productDescription, nameof(productDescription));

        return new Product(productId, productName, productDescription);
    }

    /// <summary>
    /// Updates the product name.
    /// </summary>
    /// <param name="productName">The new product name</param>
    /// <exception cref="ArgumentNullException">Thrown when productName is null</exception>
    public void UpdateName(ProductName productName)
    {
        ArgumentNullException.ThrowIfNull(productName, nameof(productName));
        Name = productName;
    }

    /// <summary>
    /// Updates the product description.
    /// </summary>
    /// <param name="productDescription">The new product description</param>
    /// <exception cref="ArgumentNullException">Thrown when productDescription is null</exception>
    public void UpdateDescription(ProductDescription productDescription)
    {
        ArgumentNullException.ThrowIfNull(productDescription, nameof(productDescription));
        Description = productDescription;
    }

    /// <summary>
    /// Determines whether the specified product is equal to the current product.
    /// Products are equal if they have the same ID.
    /// </summary>
    /// <param name="other">The product to compare with the current product</param>
    /// <returns>True if the specified product is equal to the current product; otherwise, false</returns>
    public bool Equals(Product? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current product.
    /// </summary>
    /// <param name="obj">The object to compare with the current product</param>
    /// <returns>True if the specified object is equal to the current product; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Product);
    }

    /// <summary>
    /// Returns the hash code for this product.
    /// </summary>
    /// <returns>A hash code for the current product</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the product.
    /// </summary>
    /// <returns>A string that represents the current product</returns>
    public override string ToString()
    {
        return $"Product {{ Id = {Id}, Name = \"{Name}\", Description = \"{Description}\" }}";
    }

    /// <summary>
    /// Determines whether two products are equal.
    /// </summary>
    /// <param name="left">The first product to compare</param>
    /// <param name="right">The second product to compare</param>
    /// <returns>True if the products are equal; otherwise, false</returns>
    public static bool operator ==(Product? left, Product? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two products are not equal.
    /// </summary>
    /// <param name="left">The first product to compare</param>
    /// <param name="right">The second product to compare</param>
    /// <returns>True if the products are not equal; otherwise, false</returns>
    public static bool operator !=(Product? left, Product? right)
    {
        return !(left == right);
    }
}