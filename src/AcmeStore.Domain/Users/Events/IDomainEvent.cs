using System;

namespace AcmeStore.Domain.Users.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}


