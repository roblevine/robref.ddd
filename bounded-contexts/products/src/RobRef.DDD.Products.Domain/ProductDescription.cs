namespace RobRef.DDD.Products.Domain;

/// <summary>
/// Represents a product description value object.
/// Product descriptions are optional and can be empty or null.
/// When provided, descriptions are trimmed and cannot exceed the maximum length.
/// </summary>
/// <param name="Value">The description value, trimmed and validated</param>
public record ProductDescription : IComparable<ProductDescription>
{
    /// <summary>
    /// Maximum length for a product description
    /// </summary>
    public const int MaxLength = 8095;

    /// <summary>
    /// The description value
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Initializes a new instance of ProductDescription with validation and trimming.
    /// </summary>
    /// <param name="value">The description value to validate and trim</param>
    /// <exception cref="ArgumentException">Thrown when the description exceeds maximum length</exception>
    public ProductDescription(string? value)
    {
        Value = ValidateAndTrim(value);
    }

    /// <summary>
    /// Validates and trims the product description value.
    /// </summary>
    /// <param name="value">The value to validate and trim</param>
    /// <returns>The validated and trimmed value</returns>
    /// <exception cref="ArgumentException">Thrown when the description exceeds maximum length</exception>
    private static string ValidateAndTrim(string? value)
    {
        var trimmedValue = value?.Trim() ?? string.Empty;
        
        if (trimmedValue.Length > MaxLength)
        {
            throw new ArgumentException($"Product description cannot exceed {MaxLength} characters. Provided: {trimmedValue.Length}");
        }

        return trimmedValue;
    }

    /// <summary>
    /// Compares this ProductDescription to another ProductDescription.
    /// </summary>
    /// <param name="other">The other ProductDescription to compare to</param>
    /// <returns>A value indicating the relative order of the objects</returns>
    public int CompareTo(ProductDescription? other)
    {
        if (other is null) return 1;
        return string.Compare(Value, other.Value, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns the string representation of the product description.
    /// </summary>
    /// <returns>The description value</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a string to a ProductDescription.
    /// </summary>
    /// <param name="value">The string value to convert</param>
    /// <returns>A new ProductDescription instance</returns>
    /// <exception cref="ArgumentException">Thrown when the description exceeds maximum length</exception>
    public static implicit operator ProductDescription(string? value) => new(value);

    /// <summary>
    /// Implicitly converts a ProductDescription to a string.
    /// </summary>
    /// <param name="productDescription">The ProductDescription to convert</param>
    /// <returns>The description value</returns>
    public static implicit operator string(ProductDescription productDescription) => productDescription.Value;
}