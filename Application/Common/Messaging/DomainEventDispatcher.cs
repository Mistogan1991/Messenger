using System.Collections.Concurrent;
using MediatR;
using Messenger.Domain.Common;

namespace Messenger.Application.Common.Messaging;

/// <summary>
/// Publishes each domain event through MediatR by wrapping it in a closed
/// <see cref="DomainEventNotification{TDomainEvent}"/>. The closed generic type is built once
/// per concrete event type and cached.
/// </summary>
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private static readonly ConcurrentDictionary<Type, Func<IDomainEvent, INotification>> _factories = new();

    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notification = Wrap(domainEvent);
            await _publisher.Publish(notification, ct);
        }
    }

    private static INotification Wrap(IDomainEvent domainEvent)
    {
        var factory = _factories.GetOrAdd(domainEvent.GetType(), CreateFactory);
        return factory(domainEvent);
    }

    private static Func<IDomainEvent, INotification> CreateFactory(Type eventType)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(eventType);
        return e => (INotification)Activator.CreateInstance(notificationType, e)!;
    }
}
