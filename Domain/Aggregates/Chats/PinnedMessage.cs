using Messenger.Domain.Common;

namespace Messenger.Domain.Aggregates.Chats;

public sealed class PinnedMessage : SoftDeletableEntity<Guid>
{
    public Guid ChatId { get; private set; }
    public Guid MessageId { get; private set; }
    public Guid PinnedBy { get; private set; }
    public DateTime PinnedAtUtc { get; private set; }

    private PinnedMessage() { }

    private PinnedMessage(Guid id, Guid chatId, Guid messageId, Guid pinnedBy) : base(id)
    {
        ChatId = chatId;
        MessageId = messageId;
        PinnedBy = pinnedBy;
        PinnedAtUtc = DateTime.UtcNow;
    }

    public static PinnedMessage Create(Guid chatId, Guid messageId, Guid pinnedBy)
    {
        return new PinnedMessage(Guid.NewGuid(), chatId, messageId, pinnedBy);
    }
}
