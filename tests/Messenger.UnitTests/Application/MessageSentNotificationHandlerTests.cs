using Messenger.Application.Abstractions.Realtime;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Messaging;
using Messenger.Application.Features.Messages.Dtos;
using Messenger.Application.Features.Messages.EventHandlers;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Enums;
using Messenger.Domain.Events.Messages;
using Xunit;

namespace Messenger.UnitTests.Application;

public class MessageSentNotificationHandlerTests
{
    private sealed class FakeMessageRepository : IMessageRepository
    {
        public Message? ToReturn { get; set; }
        public Task AddAsync(Message message, CancellationToken ct = default) => Task.CompletedTask;
        public Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default) => Task.FromResult(ToReturn);
        public Task<List<MessageDto>> GetChatMessagesAsync(Guid chatId, DateTime? before, int limit, CancellationToken ct = default)
            => throw new NotSupportedException();
    }

    private sealed class FakeRealtimeNotifier : IRealtimeNotifier
    {
        public Guid? NotifiedChatId { get; private set; }
        public MessageDto? Payload { get; private set; }
        public int Calls { get; private set; }
        public Task MessageSentAsync(Guid chatId, MessageDto message, CancellationToken ct = default)
        {
            Calls++;
            NotifiedChatId = chatId;
            Payload = message;
            return Task.CompletedTask;
        }
    }

    private static DomainEventNotification<MessageSentEvent> Notification(Message m) =>
        new(new MessageSentEvent(m.Id, m.ChatId, m.SenderId));

    [Fact]
    public async Task Pushes_the_message_to_the_chat_when_it_exists()
    {
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "hello");
        var notifier = new FakeRealtimeNotifier();
        var handler = new MessageSentNotificationHandler(new FakeMessageRepository { ToReturn = message }, notifier);

        await handler.Handle(Notification(message), CancellationToken.None);

        Assert.Equal(1, notifier.Calls);
        Assert.Equal(message.ChatId, notifier.NotifiedChatId);
        Assert.Equal(message.Id, notifier.Payload!.Id);
        Assert.Equal("hello", notifier.Payload.Content);
    }

    [Fact]
    public async Task Does_not_notify_when_the_message_was_not_found()
    {
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "gone");
        var notifier = new FakeRealtimeNotifier();
        var handler = new MessageSentNotificationHandler(new FakeMessageRepository { ToReturn = null }, notifier);

        await handler.Handle(Notification(message), CancellationToken.None);

        Assert.Equal(0, notifier.Calls);
    }
}
