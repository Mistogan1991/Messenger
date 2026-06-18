using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Common;
using Messenger.Domain.Common.Exceptions;
using Messenger.Domain.Enums;
using Messenger.Domain.Events.Messages;

namespace Messenger.Domain.Aggregates.Messages;

public sealed class Message : AggregateRoot<Guid>
{
    #region Properties

    public Guid ChatId { get; private set; }
    public Guid SenderId { get; private set; }
    public MessageType Type { get; private set; }
    public string? Content { get; private set; }
    public Guid? ReplyToMessageId { get; private set; }
    public Guid? ForwardedFromMessageId { get; private set; }
    public Guid? ForwardedFromUserId { get; private set; }
    public Guid? ForwardedFromChatId { get; private set; }
    public bool IsForwardAuthorHidden { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTime? EditedAtUtc { get; private set; }

    public IReadOnlyCollection<MessageAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<MessageReaction> Reactions => _reactions.AsReadOnly();

    #endregion

    private readonly List<MessageAttachment> _attachments = [];

    private readonly List<MessageReaction> _reactions = [];

    private Message() { }
    private Message(
        Guid id,
        Guid chatId,
        Guid senderId,
        MessageType type,
        string? content) : base(id)
    {
        ChatId = chatId;
        SenderId = senderId;
        Type = type;
        Content = content;
    }

    private Message(
        Guid id,
        Guid chatId,
        Guid senderId,
        MessageType type,
        string content,
        Guid replyToMessageId) : base(id)
    {
        ChatId = chatId;
        SenderId = senderId;
        Type = type;
        Content = content;
        ReplyToMessageId = replyToMessageId;
    }

    private Message(
        Guid id,
        Guid chatId,
        Guid senderId,
        Guid sourceMessageId,
        Guid sourceUserId,
        Guid sourceChatId) : base(id)
    {
        ChatId = chatId;
        SenderId = senderId;
        ForwardedFromMessageId = sourceMessageId;
        ForwardedFromUserId = sourceUserId;
        ForwardedFromChatId = sourceChatId;
    }

    #region Factory Method

    public static Message Send(Guid chatId, Guid senderId, MessageType type, string? content)
    {
        var message = new Message(Guid.NewGuid(), chatId, senderId, type, content);

        message.RaiseDomainEvent(new MessageSentEvent(message.Id, message.ChatId, message.SenderId));

        return message;
    }

    public static Message SendReply(Guid chatId, Guid senderId, Guid replyToMessageId, string content)
    {
        return new Message(Guid.NewGuid(), chatId, senderId, MessageType.Text, content, replyToMessageId);
    }

    public static Message Forward(Guid chatId, Guid senderId, Guid sourceMessageId, Guid sourceUserId, Guid sourceChatId)
    {
        return new Message(Guid.NewGuid(), chatId, senderId, sourceMessageId, sourceUserId, sourceChatId);
    }

    #endregion

    #region Domain Rules
    public void ReplyTo(Guid messageId)
    {
        ReplyToMessageId = messageId;
    }

    public void Delete()
    {
        MarkAsDelete();
        Content = null;
        RaiseDomainEvent(new MessageDeletedEvent(Id, ChatId));
    }

    public void Edit(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Message content cannot be empty.");

        Content = content;

        IsEdited = true;

        EditedAtUtc = DateTime.UtcNow;

        SetUpdated(SenderId);

        RaiseDomainEvent(new MessageEditedEvent(Id, ChatId));
    }

    public void AddAttachment(Guid fileId, string? caption)
    {
        _attachments.Add(MessageAttachment.Create(Id, fileId, caption));
    }

    public void AddReaction(Guid userId, string emoji)
    {
        if (_reactions.Any(x => x.UserId == userId && x.Emoji == emoji)) return;

        _reactions.Add(MessageReaction.Create(Id, userId, emoji));

        RaiseDomainEvent(new MessageReactionAddedEvent(Id, userId, emoji));
    }

    public void RemoveReaction(Guid userId, string emoji)
    {
        var reaction = _reactions
            .SingleOrDefault(x =>
                            x.UserId == userId &&
                            x.Emoji == emoji);

        if (reaction is null) return;

        _reactions.Remove(reaction);
    }

    #endregion
}