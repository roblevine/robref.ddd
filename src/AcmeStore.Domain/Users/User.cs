using System;
using System.Collections.Generic;
using System.Linq;
using AcmeStore.Domain.Users.Events;
using AcmeStore.Domain.Users.ValueObjects;

namespace AcmeStore.Domain.Users;

public sealed class User
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public PersonName Name { get; private set; }
    public UserStatus Status { get; private set; }

    private User(UserId id, Email email, PersonName name, UserStatus status)
    {
        Id = id;
        Email = email;
        Name = name;
        Status = status;
    }

    public static User Register(Email email, PersonName name)
    {
        var user = new User(UserId.New(), email, name, UserStatus.Active);
        user.AddEvent(new UserRegistered(user.Id));
        return user;
    }

    public void ChangeEmail(Email newEmail)
    {
        if (Status == UserStatus.Deactivated)
        {
            throw new InvalidOperationException("Cannot change email of a deactivated user.");
        }

        if (newEmail == Email)
        {
            return;
        }

        Email = newEmail;
        AddEvent(new UserEmailChanged(Id, newEmail));
    }

    public void ChangeName(PersonName newName)
    {
        if (Status == UserStatus.Deactivated)
        {
            throw new InvalidOperationException("Cannot change name of a deactivated user.");
        }

        if (newName == Name)
        {
            return;
        }

        Name = newName;
        AddEvent(new UserNameChanged(Id, newName));
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Deactivated)
        {
            return;
        }

        Status = UserStatus.Deactivated;
        AddEvent(new UserDeactivated(Id));
    }

    public void Reactivate()
    {
        if (Status == UserStatus.Active)
        {
            return;
        }

        Status = UserStatus.Active;
        AddEvent(new UserReactivated(Id));
    }

    public IReadOnlyCollection<IDomainEvent> DequeueDomainEvents()
    {
        var copy = _domainEvents.ToArray();
        _domainEvents.Clear();
        return copy;
    }

    private void AddEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }
}


