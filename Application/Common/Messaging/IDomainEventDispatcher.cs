using Messenger.Domain.Common;

namespace Messenger.Application.Common.Messaging;

/// <summary>
/// Dispatches domain events raised by aggregates to their in-process handlers.
/// Implemented in the Application layer over MediatR; invoked by the persistence layer
/// after a successful <c>SaveChanges</c> so handlers observe a committed state.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default);
}
