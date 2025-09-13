using RobRef.DDD.Domain.Users;
using RobRef.DDD.Infrastructure.Persistence;
using Xunit;

namespace RobRef.DDD.Infrastructure.Tests.Persistence;

public sealed class InMemoryUserRepositoryTests
{
    private readonly InMemoryUserRepository _repository;
    private readonly User _sampleUser;

    public InMemoryUserRepositoryTests()
    {
        _repository = new InMemoryUserRepository();
        _sampleUser = User.Register(
            new Email("test@example.com"),
            new Title("Dr"),
            new FirstName("John"),
            new LastName("Doe")
        );
    }

    [Fact]
    public async Task SaveAsync_WithValidUser_SavesSuccessfully()
    {
        // Act
        await _repository.SaveAsync(_sampleUser);

        // Assert
        var retrievedUser = await _repository.FindByIdAsync(_sampleUser.Id);
        Assert.NotNull(retrievedUser);
        Assert.Equal(_sampleUser.Id, retrievedUser!.Id);
        Assert.Equal(_sampleUser.Email, retrievedUser.Email);
    }

    [Fact]
    public async Task SaveAsync_WithNullUser_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(null!));
    }

    [Fact]
    public async Task FindByIdAsync_WithExistingId_ReturnsUser()
    {
        // Arrange
        await _repository.SaveAsync(_sampleUser);

        // Act
        var result = await _repository.FindByIdAsync(_sampleUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_sampleUser.Id, result!.Id);
    }

    [Fact]
    public async Task FindByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = UserId.NewId();

        // Act
        var result = await _repository.FindByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        await _repository.SaveAsync(_sampleUser);

        // Act
        var result = await _repository.FindByEmailAsync(_sampleUser.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_sampleUser.Email, result!.Email);
    }

    [Fact]
    public async Task FindByEmailAsync_WithNonExistentEmail_ReturnsNull()
    {
        // Arrange
        var nonExistentEmail = new Email("nonexistent@example.com");

        // Act
        var result = await _repository.FindByEmailAsync(nonExistentEmail);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        await _repository.SaveAsync(_sampleUser);

        // Act
        var result = await _repository.ExistsAsync(_sampleUser.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = UserId.NewId();

        // Act
        var result = await _repository.ExistsAsync(nonExistentId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        await _repository.SaveAsync(_sampleUser);

        // Act
        var result = await _repository.ExistsByEmailAsync(_sampleUser.Email);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithNonExistentEmail_ReturnsFalse()
    {
        // Arrange
        var nonExistentEmail = new Email("nonexistent@example.com");

        // Act
        var result = await _repository.ExistsByEmailAsync(nonExistentEmail);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_RemovesUser()
    {
        // Arrange
        await _repository.SaveAsync(_sampleUser);
        Assert.True(await _repository.ExistsAsync(_sampleUser.Id));

        // Act
        await _repository.DeleteAsync(_sampleUser);

        // Assert
        Assert.False(await _repository.ExistsAsync(_sampleUser.Id));
        Assert.Null(await _repository.FindByIdAsync(_sampleUser.Id));
    }

    [Fact]
    public async Task DeleteAsync_WithNullUser_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync(null!));
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleUsers_ReturnsAllUsers()
    {
        // Arrange
        var user2 = User.Register(
            new Email("jane@example.com"),
            null,
            new FirstName("Jane"),
            new LastName("Smith")
        );

        await _repository.SaveAsync(_sampleUser);
        await _repository.SaveAsync(user2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Id == _sampleUser.Id);
        Assert.Contains(result, u => u.Id == user2.Id);
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindByNameAsync_WithExistingName_ReturnsMatchingUsers()
    {
        // Arrange
        var user2 = User.Register(
            new Email("johndoe2@example.com"),
            new Title("Mr"),
            new FirstName("John"),
            new LastName("Doe")
        );

        await _repository.SaveAsync(_sampleUser);
        await _repository.SaveAsync(user2);

        // Act
        var result = await _repository.FindByNameAsync(_sampleUser.FirstName, _sampleUser.LastName);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, u => 
        {
            Assert.Equal(_sampleUser.FirstName, u.FirstName);
            Assert.Equal(_sampleUser.LastName, u.LastName);
        });
    }

    [Fact]
    public async Task FindByNameAsync_WithNonExistentName_ReturnsEmptyList()
    {
        // Arrange
        await _repository.SaveAsync(_sampleUser);
        var nonExistentFirstName = new FirstName("NonExistent");
        var nonExistentLastName = new LastName("Person");

        // Act
        var result = await _repository.FindByNameAsync(nonExistentFirstName, nonExistentLastName);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindByNameAsync_WithNullFirstName_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _repository.FindByNameAsync(null!, new LastName("Doe")));
    }

    [Fact]
    public async Task FindByNameAsync_WithNullLastName_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _repository.FindByNameAsync(new FirstName("John"), null!));
    }

    [Fact]
    public async Task SaveAsync_WithDuplicateUser_UpdatesExistingUser()
    {
        // Arrange
        await _repository.SaveAsync(_sampleUser);
        
        // Create new user with same ID but different email
        var updatedUser = User.Create(
            _sampleUser.Id,
            new Email("updated@example.com"),
            _sampleUser.Title,
            _sampleUser.FirstName,
            _sampleUser.LastName
        );

        // Act
        await _repository.SaveAsync(updatedUser);

        // Assert
        var retrievedUser = await _repository.FindByIdAsync(_sampleUser.Id);
        Assert.NotNull(retrievedUser);
        Assert.Equal("updated@example.com", retrievedUser!.Email.Value);

        // Should only have one user with this ID
        var allUsers = await _repository.GetAllAsync();
        Assert.Single(allUsers.Where(u => u.Id == _sampleUser.Id));
    }

    [Fact]
    public async Task Repository_IsThreadSafe_WithConcurrentOperations()
    {
        // Arrange
        const int userCount = 100;
        var users = Enumerable.Range(0, userCount)
            .Select(i => User.Register(
                new Email($"user{i}@example.com"),
                null,
                new FirstName($"User{i}"),
                new LastName("Test")))
            .ToList();

        // Act - Save users concurrently
        var saveTasks = users.Select(user => _repository.SaveAsync(user));
        await Task.WhenAll(saveTasks);

        // Assert
        var allUsers = await _repository.GetAllAsync();
        Assert.Equal(userCount, allUsers.Count);

        // Verify each user exists
        var existsTasks = users.Select(user => _repository.ExistsAsync(user.Id));
        var existsResults = await Task.WhenAll(existsTasks);
        Assert.All(existsResults, exists => Assert.True(exists));
    }
}