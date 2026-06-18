using Messenger.Domain.Events.Messages;
using Messenger.Persistence.Outbox;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class OutboxMessageTests
{
    [Fact]
    public void Create_then_Deserialize_round_trips_the_domain_event()
    {
        var original = new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var outbox = OutboxMessage.Create(original);
        var restored = Assert.IsType<MessageSentEvent>(outbox.Deserialize());

        Assert.Equal(typeof(MessageSentEvent).FullName, outbox.Type);
        Assert.Equal(original.MessageId, restored.MessageId);
        Assert.Equal(original.ChatId, restored.ChatId);
        Assert.Equal(original.SenderId, restored.SenderId);
        Assert.Equal(original.OccurredOnUtc, outbox.OccurredOnUtc);
        Assert.Null(outbox.ProcessedOnUtc);
        Assert.Equal(0, outbox.RetryCount);
    }

    [Fact]
    public void MarkFailed_increments_retry_and_keeps_message_unprocessed()
    {
        var outbox = OutboxMessage.Create(new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));

        outbox.MarkFailed("boom");

        Assert.Equal(1, outbox.RetryCount);
        Assert.Null(outbox.ProcessedOnUtc);
        Assert.Equal("boom", outbox.Error);
    }

    [Fact]
    public void MarkProcessed_sets_timestamp_and_clears_error()
    {
        var outbox = OutboxMessage.Create(new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        outbox.MarkFailed("boom");

        outbox.MarkProcessed(DateTime.UtcNow);

        Assert.NotNull(outbox.ProcessedOnUtc);
        Assert.Null(outbox.Error);
    }
}
