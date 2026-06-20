using MediatR;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Messaging;
using Messenger.Domain.Aggregates.Notifications;
using Messenger.Domain.Enums;
using Messenger.Domain.Events.Messages;

namespace Messenger.Application.Features.Notifications.EventHandlers;

/// <summary>
/// Fans a sent message out to in-app notifications: one per chat member except the sender.
/// Runs in the outbox processor's scope (at-least-once delivery).
/// </summary>
public sealed class CreateNotificationsOnMessageSentHandler
    : INotificationHandler<DomainEventNotification<MessageSentEvent>>
{
    private readonly IChatRepository _chatRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationsOnMessageSentHandler(
        IChatRepository chatRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _chatRepository = chatRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DomainEventNotification<MessageSentEvent> notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        var members = await _chatRepository.GetMembersAsync(e.ChatId, ct);

        var recipients = members.Where(m => m.UserId != e.SenderId).ToList();
        if (recipients.Count == 0)
            return;

        foreach (var recipient in recipients)
        {
            await _notificationRepository.AddAsync(
                Notification.Create(recipient.UserId, NotificationType.NewMessage, e.ChatId, e.MessageId), ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
