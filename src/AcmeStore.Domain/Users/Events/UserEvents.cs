using System;
using AcmeStore.Domain.Users.ValueObjects;

namespace AcmeStore.Domain.Users.Events;

public sealed record class UserRegistered(UserId UserId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

public sealed record class UserEmailChanged(UserId UserId, Email NewEmail) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

public sealed record class UserNameChanged(UserId UserId, PersonName NewName) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

public sealed record class UserDeactivated(UserId UserId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

public sealed record class UserReactivated(UserId UserId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}


