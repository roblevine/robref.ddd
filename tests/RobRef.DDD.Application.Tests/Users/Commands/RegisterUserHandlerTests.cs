using RobRef.DDD.Application.Users.Commands;
using RobRef.DDD.Domain.Users;
using Xunit;

namespace RobRef.DDD.Application.Tests.Users.Commands;

public sealed class RegisterUserHandlerTests
{
    private readonly InMemoryUserRepository _repository;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _repository = new InMemoryUserRepository();
        _handler = new RegisterUserHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesUserAndReturnsUserId()
    {
        // Arrange
        var command = new RegisterUser(
            Email: "john.doe@example.com",
            Title: "Mr",
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var userId = await _handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(default, userId);
        
        var savedUser = await _repository.FindByIdAsync(userId);
        Assert.NotNull(savedUser);
        Assert.Equal("john.doe@example.com", savedUser!.Email.Value);
        Assert.Equal("Mr", savedUser.Title?.Value);
        Assert.Equal("John", savedUser.FirstName.Value);
        Assert.Equal("Doe", savedUser.LastName.Value);
    }

    [Fact]
    public async Task HandleAsync_WithoutTitle_CreatesUserWithNullTitle()
    {
        // Arrange
        var command = new RegisterUser(
            Email: "jane.doe@example.com",
            Title: null,
            FirstName: "Jane",
            LastName: "Doe"
        );

        // Act
        var userId = await _handler.HandleAsync(command);

        // Assert
        var savedUser = await _repository.FindByIdAsync(userId);
        Assert.NotNull(savedUser);
        Assert.Null(savedUser!.Title);
        Assert.Equal("Jane", savedUser.FirstName.Value);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyTitle_CreatesUserWithNullTitle()
    {
        // Arrange
        var command = new RegisterUser(
            Email: "test@example.com",
            Title: "",
            FirstName: "Test",
            LastName: "User"
        );

        // Act
        var userId = await _handler.HandleAsync(command);

        // Assert
        var savedUser = await _repository.FindByIdAsync(userId);
        Assert.NotNull(savedUser);
        Assert.Null(savedUser!.Title);
    }

    [Fact]
    public async Task HandleAsync_WithWhitespaceTitle_CreatesUserWithNullTitle()
    {
        // Arrange
        var command = new RegisterUser(
            Email: "test@example.com",
            Title: "   ",
            FirstName: "Test",
            LastName: "User"
        );

        // Act
        var userId = await _handler.HandleAsync(command);

        // Assert
        var savedUser = await _repository.FindByIdAsync(userId);
        Assert.NotNull(savedUser);
        Assert.Null(savedUser!.Title);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "duplicate@example.com";
        var existingUser = User.Register(
            new Email(email),
            null,
            new FirstName("Existing"),
            new LastName("User")
        );
        await _repository.SaveAsync(existingUser);

        var command = new RegisterUser(
            Email: email,
            Title: null,
            FirstName: "New",
            LastName: "User"
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command)
        );
        Assert.Contains(email, exception.Message);
        Assert.Contains("already exists", exception.Message);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("")]
    public async Task HandleAsync_WithInvalidEmail_ThrowsArgumentException(string invalidEmail)
    {
        // Arrange
        var command = new RegisterUser(
            Email: invalidEmail,
            Title: null,
            FirstName: "Test",
            LastName: "User"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_WithInvalidFirstName_ThrowsArgumentException(string invalidFirstName)
    {
        // Arrange
        var command = new RegisterUser(
            Email: "test@example.com",
            Title: null,
            FirstName: invalidFirstName,
            LastName: "User"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_WithInvalidLastName_ThrowsArgumentException(string invalidLastName)
    {
        // Arrange
        var command = new RegisterUser(
            Email: "test@example.com",
            Title: null,
            FirstName: "Test",
            LastName: invalidLastName
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command)
        );
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserHandler(null!));
    }
}

// Simple in-memory repository for testing
public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<UserId, User> _users = new();

    public Task<User?> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email.Equals(email));
        return Task.FromResult(user);
    }

    public Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_users.ContainsKey(id));
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var exists = _users.Values.Any(u => u.Email.Equals(email));
        return Task.FromResult(exists);
    }

    public Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Remove(user.Id);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<User>>(_users.Values.ToList());
    }

    public Task<IReadOnlyList<User>> FindByNameAsync(FirstName firstName, LastName lastName, CancellationToken cancellationToken = default)
    {
        var users = _users.Values
            .Where(u => u.FirstName.Equals(firstName) && u.LastName.Equals(lastName))
            .ToList();
        return Task.FromResult<IReadOnlyList<User>>(users);
    }
}