using MediatR;
using Messenger.Application.Abstractions.Realtime;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Messaging;
using Messenger.Application.Features.Messages.Dtos;
using Messenger.Domain.Events.Messages;

namespace Messenger.Application.Features.Messages.EventHandlers;

/// <summary>
/// Reacts to a sent message (delivered via the outbox) by pushing it to the chat's members in
/// realtime. Runs in the outbox processor's scope, so delivery is at-least-once.
/// </summary>
public sealed class MessageSentNotificationHandler : INotificationHandler<DomainEventNotification<MessageSentEvent>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public MessageSentNotificationHandler(IMessageRepository messageRepository, IRealtimeNotifier realtimeNotifier)
    {
        _messageRepository = messageRepository;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task Handle(DomainEventNotification<MessageSentEvent> notification, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(notification.DomainEvent.MessageId, ct);

        if (message is null)
            return;

        var dto = new MessageDto(
            message.Id,
            message.ChatId,
            message.SenderId,
            message.Type,
            message.Content,
            message.ReplyToMessageId,
            message.IsEdited,
            message.EditedAtUtc,
            message.CreatedAtUtc);

        await _realtimeNotifier.MessageSentAsync(message.ChatId, dto, ct);
    }
}
