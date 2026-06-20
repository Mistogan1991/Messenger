using MediatR;
using Messenger.Application.Common.Messaging;
using Messenger.Contracts.IntegrationEvents;
using Messenger.Domain.Events.Messages;

namespace Messenger.Application.Features.Messages.EventHandlers;

/// <summary>
/// Bridges the in-process <see cref="MessageSentEvent"/> (delivered via the outbox) onto the
/// message bus as a <see cref="MessageSentIntegrationEvent"/> for decoupled, cross-service fan-out.
/// </summary>
public sealed class PublishMessageSentIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<MessageSentEvent>>
{
    private readonly IIntegrationEventPublisher _publisher;

    public PublishMessageSentIntegrationEventHandler(IIntegrationEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public Task Handle(DomainEventNotification<MessageSentEvent> notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        return _publisher.PublishAsync(
            new MessageSentIntegrationEvent(e.MessageId, e.ChatId, e.SenderId, e.OccurredOnUtc), ct);
    }
}
