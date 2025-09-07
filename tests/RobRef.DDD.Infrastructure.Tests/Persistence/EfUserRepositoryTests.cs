using Microsoft.EntityFrameworkCore;
using RobRef.DDD.Domain.Users;
using RobRef.DDD.Infrastructure.Persistence;

namespace RobRef.DDD.Infrastructure.Tests.Persistence;

public class EfUserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EfUserRepository _repository;

    public EfUserRepositoryTests()
    {
        // Create unique database name for each test to avoid conflicts
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EfUserRepository(_context);
    }

    [Fact]
    public async Task SaveAsync_NewUser_ShouldAddUserToDatabase()
    {
        // Arrange
        var email = new Email("john.doe@example.com");
        var title = new Title("Mr");
        var firstName = new FirstName("John");
        var lastName = new LastName("Doe");
        var user = User.Register(email, title, firstName, lastName);

        // Act
        await _repository.SaveAsync(user);

        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Id, savedUser.Id);
        Assert.Equal(user.Email.Value, savedUser.Email.Value);
        Assert.Equal(user.FirstName.Value, savedUser.FirstName.Value);
        Assert.Equal(user.LastName.Value, savedUser.LastName.Value);
        Assert.Equal(user.Title?.Value, savedUser.Title?.Value);
    }

    [Fact]
    public async Task SaveAsync_ExistingUser_ShouldUpdateUser()
    {
        // Arrange
        var email = new Email("jane.doe@example.com");
        var firstName = new FirstName("Jane");
        var lastName = new LastName("Doe");
        var user = User.Register(email, null, firstName, lastName);

        // Save initial user
        await _repository.SaveAsync(user);

        // Modify user
        var newEmail = new Email("jane.smith@example.com");
        user.ChangeEmail(newEmail);
        var newFirstName = new FirstName("Janet");
        var newLastName = new LastName("Smith");
        var title = new Title("Ms");
        user.ChangeName(title, newFirstName, newLastName);

        // Act
        await _repository.SaveAsync(user);

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newEmail.Value, updatedUser.Email.Value);
        Assert.Equal(newFirstName.Value, updatedUser.FirstName.Value);
        Assert.Equal(newLastName.Value, updatedUser.LastName.Value);
        Assert.Equal(title.Value, updatedUser.Title?.Value);
    }

    [Fact]
    public async Task FindByIdAsync_ExistingUser_ShouldReturnUser()
    {
        // Arrange
        var email = new Email("test@example.com");
        var firstName = new FirstName("Test");
        var lastName = new LastName("User");
        var user = User.Register(email, null, firstName, lastName);
        await _repository.SaveAsync(user);

        // Act
        var foundUser = await _repository.FindByIdAsync(user.Id);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
        Assert.Equal(user.Email.Value, foundUser.Email.Value);
    }

    [Fact]
    public async Task FindByIdAsync_NonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = UserId.NewId();

        // Act
        var foundUser = await _repository.FindByIdAsync(nonExistentId);

        // Assert
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task FindByEmailAsync_ExistingUser_ShouldReturnUser()
    {
        // Arrange
        var email = new Email("findme@example.com");
        var firstName = new FirstName("Find");
        var lastName = new LastName("Me");
        var user = User.Register(email, null, firstName, lastName);
        await _repository.SaveAsync(user);

        // Act
        var foundUser = await _repository.FindByEmailAsync(email);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
        Assert.Equal(email.Value, foundUser.Email.Value);
    }

    [Fact]
    public async Task FindByEmailAsync_NonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var nonExistentEmail = new Email("notfound@example.com");

        // Act
        var foundUser = await _repository.FindByEmailAsync(nonExistentEmail);

        // Assert
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task ExistsAsync_ExistingUser_ShouldReturnTrue()
    {
        // Arrange
        var email = new Email("exists@example.com");
        var firstName = new FirstName("Exists");
        var lastName = new LastName("User");
        var user = User.Register(email, null, firstName, lastName);
        await _repository.SaveAsync(user);

        // Act
        var exists = await _repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_NonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = UserId.NewId();

        // Act
        var exists = await _repository.ExistsAsync(nonExistentId);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task ExistsByEmailAsync_ExistingUser_ShouldReturnTrue()
    {
        // Arrange
        var email = new Email("emailexists@example.com");
        var firstName = new FirstName("Email");
        var lastName = new LastName("Exists");
        var user = User.Register(email, null, firstName, lastName);
        await _repository.SaveAsync(user);

        // Act
        var exists = await _repository.ExistsByEmailAsync(email);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByEmailAsync_NonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentEmail = new Email("emailnotfound@example.com");

        // Act
        var exists = await _repository.ExistsByEmailAsync(nonExistentEmail);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_ShouldRemoveUser()
    {
        // Arrange
        var email = new Email("delete@example.com");
        var firstName = new FirstName("Delete");
        var lastName = new LastName("Me");
        var user = User.Register(email, null, firstName, lastName);
        await _repository.SaveAsync(user);

        // Act
        await _repository.DeleteAsync(user);

        // Assert
        var deletedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ShouldNotThrow()
    {
        // Arrange
        var email = new Email("nonexistent@example.com");
        var firstName = new FirstName("Non");
        var lastName = new LastName("Existent");
        var user = User.Register(email, null, firstName, lastName);

        // Act & Assert - should not throw
        await _repository.DeleteAsync(user);
    }

    [Fact]
    public async Task GetAllAsync_MultipleUsers_ShouldReturnAllOrderedByName()
    {
        // Arrange
        var user1 = User.Register(new Email("alice@example.com"), null, new FirstName("Alice"), new LastName("Smith"));
        var user2 = User.Register(new Email("bob@example.com"), null, new FirstName("Bob"), new LastName("Johnson"));
        var user3 = User.Register(new Email("charlie@example.com"), null, new FirstName("Charlie"), new LastName("Brown"));

        await _repository.SaveAsync(user1);
        await _repository.SaveAsync(user2);
        await _repository.SaveAsync(user3);

        // Act
        var users = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, users.Count);
        // Should be ordered by LastName, then FirstName
        Assert.Equal("Brown", users[0].LastName.Value);      // Charlie Brown
        Assert.Equal("Johnson", users[1].LastName.Value);    // Bob Johnson
        Assert.Equal("Smith", users[2].LastName.Value);      // Alice Smith
    }

    [Fact]
    public async Task GetAllAsync_NoUsers_ShouldReturnEmptyList()
    {
        // Act
        var users = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public async Task FindByNameAsync_ExistingUser_ShouldReturnUsers()
    {
        // Arrange
        var firstName = new FirstName("John");
        var lastName = new LastName("Doe");
        var user1 = User.Register(new Email("john1@example.com"), null, firstName, lastName);
        var user2 = User.Register(new Email("john2@example.com"), new Title("Dr"), firstName, lastName);
        var user3 = User.Register(new Email("jane@example.com"), null, new FirstName("Jane"), lastName);

        await _repository.SaveAsync(user1);
        await _repository.SaveAsync(user2);
        await _repository.SaveAsync(user3);

        // Act
        var users = await _repository.FindByNameAsync(firstName, lastName);

        // Assert
        Assert.Equal(2, users.Count);
        Assert.All(users, u => Assert.Equal("John", u.FirstName.Value));
        Assert.All(users, u => Assert.Equal("Doe", u.LastName.Value));
        // Should be ordered by email
        Assert.Equal("john1@example.com", users[0].Email.Value);
        Assert.Equal("john2@example.com", users[1].Email.Value);
    }

    [Fact]
    public async Task FindByNameAsync_NoMatchingUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var user = User.Register(new Email("test@example.com"), null, new FirstName("Test"), new LastName("User"));
        await _repository.SaveAsync(user);

        var searchFirstName = new FirstName("NonExistent");
        var searchLastName = new LastName("Person");

        // Act
        var users = await _repository.FindByNameAsync(searchFirstName, searchLastName);

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public async Task SaveAsync_DuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var email = new Email("duplicate@example.com");
        var user1 = User.Register(email, null, new FirstName("User1"), new LastName("Test"));
        var user2 = User.Register(email, null, new FirstName("User2"), new LastName("Test"));

        await _repository.SaveAsync(user1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.SaveAsync(user2));
    }

    [Fact]
    public async Task Repository_ValueObjectConversions_ShouldWorkCorrectly()
    {
        // Arrange
        var email = new Email("conversion@example.com");
        var title = new Title("Dr");
        var firstName = new FirstName("Conversion");
        var lastName = new LastName("Test");
        var user = User.Register(email, title, firstName, lastName);

        // Act
        await _repository.SaveAsync(user);
        
        // Clear the change tracker to ensure we get a fresh instance
        _context.ChangeTracker.Clear();
        
        var retrievedUser = await _repository.FindByIdAsync(user.Id);

        // Assert
        Assert.NotNull(retrievedUser);
        
        // Test that value objects are properly converted
        Assert.Equal(email.Value, retrievedUser.Email.Value);
        Assert.Equal(title.Value, retrievedUser.Title?.Value);
        Assert.Equal(firstName.Value, retrievedUser.FirstName.Value);
        Assert.Equal(lastName.Value, retrievedUser.LastName.Value);
        
        // Test that they're different instances (not reference equality) after clearing change tracker
        Assert.NotSame(user, retrievedUser);
        
        // But equivalent (value equality)
        Assert.Equal(user, retrievedUser);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}