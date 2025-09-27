namespace RobRef.DDD.Products.Domain;

/// <summary>
/// Repository interface for managing Product aggregates.
/// Defines the contract for persistence operations on Product entities.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The unique product identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The product if found, otherwise null</returns>
    Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A collection of all products</returns>
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new product to the repository.
    /// </summary>
    /// <param name="product">The product to add</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product in the repository.
    /// </summary>
    /// <param name="product">The product to update</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a product from the repository.
    /// </summary>
    /// <param name="product">The product to remove</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a product with the specified identifier exists.
    /// </summary>
    /// <param name="productId">The unique product identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if the product exists, otherwise false</returns>
    Task<bool> ExistsAsync(ProductId productId, CancellationToken cancellationToken = default);
}