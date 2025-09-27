namespace RobRef.DDD.Products.Domain;

/// <summary>
/// Represents a product name with validation constraints.
/// </summary>
public record class ProductName(string Value) : IComparable<ProductName>
{
    /// <summary>
    /// The maximum length allowed for a product name.
    /// </summary>
    public const int MaxLength = 254;

    /// <summary>
    /// Gets the trimmed and validated product name value.
    /// </summary>
    public string Value { get; } = ValidateAndTrim(Value);

    /// <summary>
    /// Validates and trims the product name value.
    /// </summary>
    private static string ValidateAndTrim(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "Product name cannot be null");

        var trimmedValue = value.Trim();
        
        if (string.IsNullOrEmpty(trimmedValue))
            throw new ArgumentException("Product name cannot be empty or whitespace", nameof(value));

        if (trimmedValue.Length > MaxLength)
            throw new ArgumentException($"Product name cannot exceed {MaxLength} characters", nameof(value));

        return trimmedValue;
    }

    /// <summary>
    /// Compares this ProductName with another ProductName based on their string values.
    /// </summary>
    public int CompareTo(ProductName? other)
    {
        if (other is null) return 1;
        return string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns the string representation of the product name.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a ProductName to its string representation.
    /// </summary>
    public static implicit operator string(ProductName productName) => productName.Value;

    /// <summary>
    /// Implicitly converts a string to a ProductName with validation.
    /// </summary>
    public static implicit operator ProductName(string value) => new(value);
}