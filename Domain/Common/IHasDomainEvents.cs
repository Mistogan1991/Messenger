namespace Messenger.Domain.Common;

/// <summary>
/// Non-generic abstraction over an aggregate root that records domain events.
/// Lets the persistence layer collect/clear events without knowing the aggregate's id type.
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
