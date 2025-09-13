using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Register_ValidData_CreatesUser()
    {
        // Arrange
        var email = new Email("john.doe@example.com");
        var title = new Title("Mr");
        var firstName = new FirstName("John");
        var lastName = new LastName("Doe");

        // Act
        var user = User.Register(email, title, firstName, lastName);

        // Assert
        Assert.NotEqual(default(UserId), user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(title, user.Title);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
    }

    [Fact]
    public void Register_NullTitle_CreatesUser()
    {
        // Arrange
        var email = new Email("jane.doe@example.com");
        var firstName = new FirstName("Jane");
        var lastName = new LastName("Doe");

        // Act
        var user = User.Register(email, null, firstName, lastName);

        // Assert
        Assert.NotEqual(default(UserId), user.Id);
        Assert.Equal(email, user.Email);
        Assert.Null(user.Title);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
    }

    [Fact]
    public void Register_NullEmail_ThrowsArgumentNullException()
    {
        // Arrange
        var firstName = new FirstName("John");
        var lastName = new LastName("Doe");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            User.Register(null!, null, firstName, lastName));
    }

    [Fact]
    public void Register_NullFirstName_ThrowsArgumentNullException()
    {
        // Arrange
        var email = new Email("john.doe@example.com");
        var lastName = new LastName("Doe");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            User.Register(email, null, null!, lastName));
    }

    [Fact]
    public void Register_NullLastName_ThrowsArgumentNullException()
    {
        // Arrange
        var email = new Email("john.doe@example.com");
        var firstName = new FirstName("John");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            User.Register(email, null, firstName, null!));
    }

    [Fact]
    public void ChangeEmail_ValidEmail_UpdatesEmail()
    {
        // Arrange
        var user = User.Register(
            new Email("old@example.com"),
            null,
            new FirstName("John"),
            new LastName("Doe"));
        var newEmail = new Email("new@example.com");

        // Act
        user.ChangeEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void ChangeEmail_NullEmail_ThrowsArgumentNullException()
    {
        // Arrange
        var user = User.Register(
            new Email("old@example.com"),
            null,
            new FirstName("John"),
            new LastName("Doe"));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => user.ChangeEmail(null!));
    }

    [Fact]
    public void ChangeName_ValidData_UpdatesName()
    {
        // Arrange
        var user = User.Register(
            new Email("john.doe@example.com"),
            new Title("Mr"),
            new FirstName("John"),
            new LastName("Doe"));

        var newTitle = new Title("Dr");
        var newFirstName = new FirstName("Jonathan");
        var newLastName = new LastName("Smith");

        // Act
        user.ChangeName(newTitle, newFirstName, newLastName);

        // Assert
        Assert.Equal(newTitle, user.Title);
        Assert.Equal(newFirstName, user.FirstName);
        Assert.Equal(newLastName, user.LastName);
    }

    [Fact]
    public void ChangeName_NullTitle_UpdatesName()
    {
        // Arrange
        var user = User.Register(
            new Email("john.doe@example.com"),
            new Title("Mr"),
            new FirstName("John"),
            new LastName("Doe"));

        var newFirstName = new FirstName("Jonathan");
        var newLastName = new LastName("Smith");

        // Act
        user.ChangeName(null, newFirstName, newLastName);

        // Assert
        Assert.Null(user.Title);
        Assert.Equal(newFirstName, user.FirstName);
        Assert.Equal(newLastName, user.LastName);
    }

    [Fact]
    public void ChangeName_NullFirstName_ThrowsArgumentNullException()
    {
        // Arrange
        var user = User.Register(
            new Email("john.doe@example.com"),
            null,
            new FirstName("John"),
            new LastName("Doe"));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            user.ChangeName(null, null!, new LastName("Smith")));
    }

    [Fact]
    public void ChangeName_NullLastName_ThrowsArgumentNullException()
    {
        // Arrange
        var user = User.Register(
            new Email("john.doe@example.com"),
            null,
            new FirstName("John"),
            new LastName("Doe"));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            user.ChangeName(null, new FirstName("John"), null!));
    }

    [Fact]
    public void Equality_SameId_AreEqual()
    {
        // Arrange
        var id = UserId.NewId();
        var user1 = CreateUserWithId(id);
        var user2 = CreateUserWithId(id);

        // Act & Assert
        Assert.Equal(user1, user2);
        Assert.True(user1 == user2);
        Assert.False(user1 != user2);
        Assert.Equal(user1.GetHashCode(), user2.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentId_AreNotEqual()
    {
        // Arrange
        var user1 = User.Register(
            new Email("user1@example.com"),
            null,
            new FirstName("User"),
            new LastName("One"));
        var user2 = User.Register(
            new Email("user2@example.com"),
            null,
            new FirstName("User"),
            new LastName("Two"));

        // Act & Assert
        Assert.NotEqual(user1, user2);
        Assert.False(user1 == user2);
        Assert.True(user1 != user2);
    }

    private static User CreateUserWithId(UserId id)
    {
        return User.Create(
            id,
            new Email($"test_{id}@example.com"),
            null,
            new FirstName("Test"),
            new LastName("User"));
    }
}