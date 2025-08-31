using System;

namespace AcmeStore.Domain.Tests.Users;

public class UserAggregateTests
{
    [Fact]
    public void Register_CreatesActiveUser_EmitsUserRegistered()
    {
        var email = AcmeStore.Domain.Users.ValueObjects.Email.Create("alice@example.com");
        var name = AcmeStore.Domain.Users.ValueObjects.PersonName.Create(null, "Alice", "Anderson");

        var user = AcmeStore.Domain.Users.User.Register(email, name);

        Assert.NotEqual(default, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.Equal(AcmeStore.Domain.Users.UserStatus.Active, user.Status);
        Assert.Contains(user.DequeueDomainEvents(), e => e is AcmeStore.Domain.Users.Events.UserRegistered);
    }

    [Fact]
    public void ChangeEmail_UpdatesEmail_EmitsEvent_NoOpWhenSame()
    {
        var user = NewUser();
        var newEmail = AcmeStore.Domain.Users.ValueObjects.Email.Create("alice.new@example.com");

        user.ChangeEmail(newEmail);
        Assert.Equal(newEmail, user.Email);
        Assert.Contains(user.DequeueDomainEvents(), e => e is AcmeStore.Domain.Users.Events.UserEmailChanged);

        user.ChangeEmail(newEmail);
        Assert.Empty(user.DequeueDomainEvents());
    }

    [Fact]
    public void ChangeName_UpdatesName_EmitsEvent_NoOpWhenSame()
    {
        var user = NewUser();
        var newName = AcmeStore.Domain.Users.ValueObjects.PersonName.Create("Dr", "Alice", "Anderson");

        user.ChangeName(newName);
        Assert.Equal(newName, user.Name);
        Assert.Contains(user.DequeueDomainEvents(), e => e is AcmeStore.Domain.Users.Events.UserNameChanged);

        user.ChangeName(newName);
        Assert.Empty(user.DequeueDomainEvents());
    }

    [Fact]
    public void Deactivate_Then_Reactivate_Idempotent_And_EmitsEvents()
    {
        var user = NewUser();

        user.Deactivate();
        Assert.Equal(AcmeStore.Domain.Users.UserStatus.Deactivated, user.Status);
        Assert.Contains(user.DequeueDomainEvents(), e => e is AcmeStore.Domain.Users.Events.UserDeactivated);

        user.Deactivate();
        Assert.Empty(user.DequeueDomainEvents());

        user.Reactivate();
        Assert.Equal(AcmeStore.Domain.Users.UserStatus.Active, user.Status);
        Assert.Contains(user.DequeueDomainEvents(), e => e is AcmeStore.Domain.Users.Events.UserReactivated);

        user.Reactivate();
        Assert.Empty(user.DequeueDomainEvents());
    }

    private static AcmeStore.Domain.Users.User NewUser()
    {
        var email = AcmeStore.Domain.Users.ValueObjects.Email.Create("alice@example.com");
        var name = AcmeStore.Domain.Users.ValueObjects.PersonName.Create(null, "Alice", "Anderson");
        return AcmeStore.Domain.Users.User.Register(email, name);
    }
}


