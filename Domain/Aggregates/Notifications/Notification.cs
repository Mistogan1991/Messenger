using Messenger.Domain.Common;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Aggregates.Notifications;

/// <summary>An in-app notification delivered to a single recipient.</summary>
public sealed class Notification : AggregateRoot<Guid>
{
    public Guid RecipientUserId { get; private set; }
    public NotificationType Type { get; private set; }
    public Guid? ChatId { get; private set; }
    public Guid? MessageId { get; private set; }
    public bool IsRead { get; private set; }

    private Notification() { }

    private Notification(Guid id, Guid recipientUserId, NotificationType type, Guid? chatId, Guid? messageId)
        : base(id)
    {
        RecipientUserId = recipientUserId;
        Type = type;
        ChatId = chatId;
        MessageId = messageId;
    }

    public static Notification Create(
        Guid recipientUserId, NotificationType type, Guid? chatId = null, Guid? messageId = null)
        => new(Guid.NewGuid(), recipientUserId, type, chatId, messageId);

    public void MarkAsRead()
    {
        if (IsRead) return;

        IsRead = true;
        SetUpdated(RecipientUserId);
    }
}
