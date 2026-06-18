using Messenger.Domain.Common;

namespace Messenger.Domain.Aggregates.Messages;

public sealed class MessageReaction : SoftDeletableEntity<Guid>
{
    public Guid MessageId { get; private set; }
    public Guid UserId { get; private set; }
    public string Emoji { get; private set; }

    private MessageReaction() { }

    private MessageReaction(Guid id, Guid messageId, Guid userId, string emoji) : base(id)
    {
        MessageId = messageId;
        UserId = userId;
        Emoji = emoji;
    }

    public static MessageReaction Create(Guid messageId, Guid userId, string emoji)
    {
        return new MessageReaction(Guid.NewGuid(), messageId, userId, emoji);
    }
}
