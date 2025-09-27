namespace RobRef.DDD.Products.Domain;

/// <summary>
/// Represents a unique product identifier using ULID for sortable, time-based unique identifiers.
/// </summary>
public readonly record struct ProductId(Ulid Value) : IComparable<ProductId>
{
    /// <summary>
    /// Gets an empty ProductId with ULID.Empty value.
    /// </summary>
    public static ProductId Empty => new(Ulid.Empty);

    /// <summary>
    /// Creates a new ProductId with a generated ULID value.
    /// </summary>
    public static ProductId Create() => new(Ulid.NewUlid());

    /// <summary>
    /// Parses a string representation of a ULID into a ProductId.
    /// </summary>
    /// <param name="value">The ULID string to parse.</param>
    /// <returns>A ProductId with the parsed ULID value.</returns>
    /// <exception cref="ArgumentException">Thrown when the input string is not a valid ULID.</exception>
    public static ProductId Parse(string value) => new(Ulid.Parse(value));

    /// <summary>
    /// Tries to parse a string representation of a ULID into a ProductId.
    /// </summary>
    /// <param name="value">The ULID string to parse.</param>
    /// <param name="productId">The parsed ProductId if successful, otherwise Empty.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParse(string? value, out ProductId productId)
    {
        if (Ulid.TryParse(value, out var ulid))
        {
            productId = new(ulid);
            return true;
        }

        productId = Empty;
        return false;
    }

    /// <summary>
    /// Compares this ProductId with another ProductId based on their ULID values.
    /// </summary>
    public int CompareTo(ProductId other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Returns the string representation of the underlying ULID.
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicitly converts a ProductId to its string representation.
    /// </summary>
    public static implicit operator string(ProductId productId) => productId.ToString();

    /// <summary>
    /// Implicitly converts a string to a ProductId by parsing the ULID.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the input string is not a valid ULID.</exception>
    public static implicit operator ProductId(string value) => Parse(value);
}