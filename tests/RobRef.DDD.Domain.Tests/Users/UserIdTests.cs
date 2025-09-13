using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Domain.Tests.Users;

public class UserIdTests
{
    [Fact]
    public void Constructor_ValidUlid_CreatesInstance()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        
        // Act
        var userId = new UserId(ulid);
        
        // Assert
        Assert.Equal(ulid, userId.Value);
    }

    [Fact]
    public void NewId_CreatesUniqueInstances()
    {
        // Act
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        
        // Assert
        Assert.NotEqual(userId1, userId2);
        Assert.NotEqual(userId1.Value, userId2.Value);
    }

    [Fact]
    public void NewId_CreatesValidUlids()
    {
        // Act
        var userId = UserId.NewId();
        
        // Assert
        Assert.NotEqual(default(Ulid), userId.Value);
        Assert.True(userId.Value.Time > DateTimeOffset.MinValue);
    }

    [Fact]
    public void Equality_SameUlidValues_AreEqual()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var userId1 = new UserId(ulid);
        var userId2 = new UserId(ulid);
        
        // Act & Assert
        Assert.Equal(userId1, userId2);
        Assert.True(userId1 == userId2);
        Assert.False(userId1 != userId2);
        Assert.Equal(userId1.GetHashCode(), userId2.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentUlidValues_AreNotEqual()
    {
        // Arrange
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        
        // Act & Assert
        Assert.NotEqual(userId1, userId2);
        Assert.False(userId1 == userId2);
        Assert.True(userId1 != userId2);
    }

    [Fact]
    public void ToString_ReturnsUlidString()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var userId = new UserId(ulid);
        
        // Act
        var result = userId.ToString();
        
        // Assert
        Assert.Equal(ulid.ToString(), result);
    }

    [Fact]
    public void Parse_ValidUlidString_CreatesUserId()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var ulidString = ulid.ToString();
        
        // Act
        var userId = UserId.Parse(ulidString);
        
        // Assert
        Assert.Equal(ulid, userId.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Parse_NullOrEmptyString_ThrowsFormatException(string invalidString)
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => UserId.Parse(invalidString));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("123")]
    public void Parse_InvalidUlidFormat_ThrowsArgumentException(string invalidString)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => UserId.Parse(invalidString));
    }

    [Fact]
    public void TryParse_ValidString_ReturnsTrue()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        var ulidString = ulid.ToString();
        
        // Act
        var success = UserId.TryParse(ulidString, out var userId);
        
        // Assert
        Assert.True(success);
        Assert.Equal(ulid, userId.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public void TryParse_InvalidString_ReturnsFalse(string invalidString)
    {
        // Act
        var success = UserId.TryParse(invalidString, out var userId);
        
        // Assert
        Assert.False(success);
        Assert.Equal(default(UserId), userId);
    }

    [Fact]
    public void ImplicitConversion_FromUlid_CreatesUserId()
    {
        // Arrange
        var ulid = Ulid.NewUlid();
        
        // Act
        UserId userId = ulid;
        
        // Assert
        Assert.Equal(ulid, userId.Value);
    }

    [Fact]
    public void ImplicitConversion_ToUlid_ReturnsValue()
    {
        // Arrange
        var userId = UserId.NewId();
        
        // Act
        Ulid ulid = userId;
        
        // Assert
        Assert.Equal(userId.Value, ulid);
    }
}