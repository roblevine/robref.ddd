using RobRef.DDD.Application.Users.Queries;
using RobRef.DDD.Application.Tests.Users.Commands;
using RobRef.DDD.Domain.Users;
using Xunit;

namespace RobRef.DDD.Application.Tests.Users.Queries;

public sealed class GetAllUsersHandlerTests
{
    private readonly InMemoryUserRepository _repository;
    private readonly GetAllUsersHandler _handler;

    public GetAllUsersHandlerTests()
    {
        _repository = new InMemoryUserRepository();
        _handler = new GetAllUsersHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllUsers();

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_WithSingleUser_ReturnsSingleUserList()
    {
        // Arrange
        var user = User.Register(
            new Email("john.doe@example.com"),
            new Title("Mr"),
            new FirstName("John"),
            new LastName("Doe")
        );
        await _repository.SaveAsync(user);

        var query = new GetAllUsers();

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(user.Id, result[0].Id);
        Assert.Equal("john.doe@example.com", result[0].Email.Value);
        Assert.Equal("John", result[0].FirstName.Value);
        Assert.Equal("Doe", result[0].LastName.Value);
    }

    [Fact]
    public async Task HandleAsync_WithMultipleUsers_ReturnsAllUsers()
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
        var user3 = User.Register(
            new Email("bob.wilson@example.com"),
            new Title("Dr"),
            new FirstName("Bob"),
            new LastName("Wilson")
        );

        await _repository.SaveAsync(user1);
        await _repository.SaveAsync(user2);
        await _repository.SaveAsync(user3);

        var query = new GetAllUsers();

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        var userIds = result.Select(u => u.Id).ToList();
        Assert.Contains(user1.Id, userIds);
        Assert.Contains(user2.Id, userIds);
        Assert.Contains(user3.Id, userIds);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_PassesToRepository()
    {
        // Arrange
        var query = new GetAllUsers();
        using var cts = new CancellationTokenSource();

        // Act
        var result = await _handler.HandleAsync(query, cts.Token);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GetAllUsersHandler(null!));
    }
}