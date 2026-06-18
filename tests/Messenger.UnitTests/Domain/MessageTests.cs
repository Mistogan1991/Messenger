using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Enums;
using Messenger.Domain.Events.Messages;
using Xunit;

namespace Messenger.UnitTests.Domain;

public class MessageTests
{
    [Fact]
    public void Send_raises_MessageSentEvent()
    {
        var chatId = Guid.NewGuid();
        var senderId = Guid.NewGuid();

        var message = Message.Send(chatId, senderId, MessageType.Text, "hello");

        var evt = Assert.Single(message.DomainEvents);
        var sent = Assert.IsType<MessageSentEvent>(evt);
        Assert.Equal(message.Id, sent.MessageId);
        Assert.Equal(chatId, sent.ChatId);
        Assert.Equal(senderId, sent.SenderId);
    }

    [Fact]
    public void Edit_raises_MessageEditedEvent_and_marks_edited()
    {
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "hello");
        message.ClearDomainEvents();

        message.Edit("updated");

        Assert.True(message.IsEdited);
        Assert.Equal("updated", message.Content);
        Assert.Contains(message.DomainEvents, e => e is MessageEditedEvent);
    }

    [Fact]
    public void Delete_clears_content_and_raises_MessageDeletedEvent()
    {
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "hello");
        message.ClearDomainEvents();

        message.Delete();

        Assert.True(message.IsDeleted);
        Assert.Null(message.Content);
        Assert.Contains(message.DomainEvents, e => e is MessageDeletedEvent);
    }

    [Fact]
    public void AddReaction_is_idempotent_and_raises_event_once()
    {
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "hello");
        message.ClearDomainEvents();
        var userId = Guid.NewGuid();

        message.AddReaction(userId, "👍");
        message.AddReaction(userId, "👍");

        Assert.Single(message.Reactions);
        Assert.Single(message.DomainEvents, e => e is MessageReactionAddedEvent);
    }
}
