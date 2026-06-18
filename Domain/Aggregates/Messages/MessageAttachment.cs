using Messenger.Domain.Common;

namespace Messenger.Domain.Aggregates.Messages;

public sealed class MessageAttachment : SoftDeletableEntity<Guid>
{
    public Guid MessageId { get; private set; }
    public Guid FileId { get; private set; }
    public string? Caption { get; private set; }

    private MessageAttachment() { }

    private MessageAttachment(Guid id, Guid messageId, Guid fileId, string? caption) : base(id)
    {
        MessageId = messageId;
        FileId = fileId;
        Caption = caption;
    }

    public static MessageAttachment Create(Guid messageId, Guid fileId, string? caption)
    {
        return new MessageAttachment(Guid.NewGuid(), messageId, fileId, caption);
    }
}