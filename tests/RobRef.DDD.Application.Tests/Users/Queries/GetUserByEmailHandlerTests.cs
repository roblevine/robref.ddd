using RobRef.DDD.Application.Users.Queries;
using RobRef.DDD.Application.Tests.Users.Commands;
using RobRef.DDD.Domain.Users;
using Xunit;

namespace RobRef.DDD.Application.Tests.Users.Queries;

public sealed class GetUserByEmailHandlerTests
{
    private readonly InMemoryUserRepository _repository;
    private readonly GetUserByEmailHandler _handler;

    public GetUserByEmailHandlerTests()
    {
        _repository = new InMemoryUserRepository();
        _handler = new GetUserByEmailHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var email = "john.doe@example.com";
        var user = User.Register(
            new Email(email),
            new Title("Mr"),
            new FirstName("John"),
            new LastName("Doe")
        );
        await _repository.SaveAsync(user);

        var query = new GetUserByEmail(email);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal(email, result.Email.Value);
        Assert.Equal("Mr", result.Title?.Value);
        Assert.Equal("John", result.FirstName.Value);
        Assert.Equal("Doe", result.LastName.Value);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Arrange
        var query = new GetUserByEmail("nonexistent@example.com");

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_WithDifferentCaseEmail_ReturnsNull()
    {
        // Arrange
        var email = "john.doe@example.com";
        var user = User.Register(
            new Email(email),
            null,
            new FirstName("John"),
            new LastName("Doe")
        );
        await _repository.SaveAsync(user);

        var query = new GetUserByEmail("JOHN.DOE@EXAMPLE.COM");

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_WithMultipleUsers_ReturnsCorrectUser()
    {
        // Arrange
        var user1 = User.Register(
            new Email("john.doe@example.com"),
            new Title("Mr"),
            new FirstName("John"),
            new LastName("Doe")
        );
        var user2 = User.Register(
            new Email("jane.smith@example.com"),
            null,
            new FirstName("Jane"),
            new LastName("Smith")
        );

        await _repository.SaveAsync(user1);
        await _repository.SaveAsync(user2);

        var query = new GetUserByEmail("jane.smith@example.com");

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user2.Id, result!.Id);
        Assert.Equal("jane.smith@example.com", result.Email.Value);
        Assert.Equal("Jane", result.FirstName.Value);
        Assert.Equal("Smith", result.LastName.Value);
        Assert.Null(result.Title);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_PassesToRepository()
    {
        // Arrange
        var query = new GetUserByEmail("test@example.com");
        using var cts = new CancellationTokenSource();

        // Act
        var result = await _handler.HandleAsync(query, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("")]
    public async Task HandleAsync_WithInvalidEmail_ThrowsArgumentException(string invalidEmail)
    {
        // Arrange
        var query = new GetUserByEmail(invalidEmail);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(query)
        );
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GetUserByEmailHandler(null!));
    }
}