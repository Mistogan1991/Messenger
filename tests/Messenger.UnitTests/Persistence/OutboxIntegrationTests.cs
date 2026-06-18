using Messenger.Application.Common.Messaging;
using Messenger.Domain.Common;
using Messenger.Domain.Enums;
using Messenger.Domain.Events.Messages;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Persistence.Context;
using Messenger.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class OutboxIntegrationTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"outbox-{Guid.NewGuid()}")
            .Options);

    private sealed class CapturingDispatcher : IDomainEventDispatcher
    {
        public List<IDomainEvent> Dispatched { get; } = [];

        public Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default)
        {
            Dispatched.AddRange(domainEvents);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default) =>
            throw new InvalidOperationException("handler failed");
    }

    private static OutboxProcessor NewProcessor(OutboxOptions? options = null) =>
        new(scopeFactory: null!, Options.Create(options ?? new OutboxOptions()), NullLogger<OutboxProcessor>.Instance);

    [Fact]
    public async Task SaveChanges_writes_one_outbox_row_per_domain_event_and_clears_events()
    {
        await using var db = NewContext();
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "hi");
        db.Messages.Add(message);

        await db.SaveChangesAsync();

        var rows = await db.OutboxMessages.ToListAsync();
        Assert.Single(rows);
        Assert.Equal(typeof(MessageSentEvent).FullName, rows[0].Type);
        Assert.Empty(message.DomainEvents);
    }

    [Fact]
    public async Task ProcessBatch_publishes_pending_messages_and_marks_them_processed()
    {
        await using var db = NewContext();
        db.OutboxMessages.Add(OutboxMessage.Create(
            new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())));
        await db.SaveChangesAsync();

        var dispatcher = new CapturingDispatcher();
        var processed = await NewProcessor().ProcessBatchAsync(db, dispatcher, CancellationToken.None);

        Assert.Equal(1, processed);
        Assert.Single(dispatcher.Dispatched);
        Assert.IsType<MessageSentEvent>(dispatcher.Dispatched[0]);
        Assert.All(await db.OutboxMessages.ToListAsync(), m => Assert.NotNull(m.ProcessedOnUtc));
    }

    [Fact]
    public async Task ProcessBatch_on_failure_increments_retry_and_leaves_message_pending()
    {
        await using var db = NewContext();
        db.OutboxMessages.Add(OutboxMessage.Create(
            new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())));
        await db.SaveChangesAsync();

        await NewProcessor().ProcessBatchAsync(db, new ThrowingDispatcher(), CancellationToken.None);

        var row = Assert.Single(await db.OutboxMessages.ToListAsync());
        Assert.Null(row.ProcessedOnUtc);
        Assert.Equal(1, row.RetryCount);
        Assert.Contains("handler failed", row.Error);
    }

    [Fact]
    public async Task ProcessBatch_skips_messages_that_exhausted_their_retries()
    {
        var options = new OutboxOptions { MaxRetries = 1 };
        await using var db = NewContext();
        var outbox = OutboxMessage.Create(new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        outbox.MarkFailed("previous failure"); // RetryCount = 1, equals MaxRetries
        db.OutboxMessages.Add(outbox);
        await db.SaveChangesAsync();

        var dispatcher = new CapturingDispatcher();
        var processed = await NewProcessor(options).ProcessBatchAsync(db, dispatcher, CancellationToken.None);

        Assert.Equal(0, processed);
        Assert.Empty(dispatcher.Dispatched);
    }
}
