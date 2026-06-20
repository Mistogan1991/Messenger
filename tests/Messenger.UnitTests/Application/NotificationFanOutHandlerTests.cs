using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Messaging;
using Messenger.Application.Features.Chats.Dtos;
using Messenger.Application.Features.Notifications.Dtos;
using Messenger.Application.Features.Notifications.EventHandlers;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Notifications;
using Messenger.Domain.Enums;
using Messenger.Domain.Events.Messages;
using Xunit;

namespace Messenger.UnitTests.Application;

public class NotificationFanOutHandlerTests
{
    private sealed class FakeChatRepository : IChatRepository
    {
        public List<ChatMemberDto> Members { get; set; } = [];
        public Task<List<ChatMemberDto>> GetMembersAsync(Guid chatId, CancellationToken ct) => Task.FromResult(Members);

        public Task AddAsync(Chat chat, CancellationToken ct) => throw new NotSupportedException();
        public Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<Chat?> GetPrivateChatAsync(Guid a, Guid b, CancellationToken ct) => throw new NotSupportedException();
        public Task<List<MyChatDto>> GetUserChatsAsync(Guid userId, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> IsParticipantAsync(Guid chatId, Guid userId, CancellationToken ct) => throw new NotSupportedException();
    }

    private sealed class FakeNotificationRepository : INotificationRepository
    {
        public List<Notification> Added { get; } = [];
        public Task AddAsync(Notification notification, CancellationToken ct = default) { Added.Add(notification); return Task.CompletedTask; }
        public Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<List<NotificationDto>> GetForUserAsync(Guid userId, bool unreadOnly, int limit, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<int> CountUnreadAsync(Guid userId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<int> MarkAllReadAsync(Guid userId, CancellationToken ct = default) => throw new NotSupportedException();
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }
        public Task<int> SaveChangesAsync(CancellationToken ct = default) { SaveCount++; return Task.FromResult(1); }
    }

    private static ChatMemberDto Member(Guid id) => new(id, null, null, null, ChatRole.Member);

    [Fact]
    public async Task Creates_a_notification_for_every_member_except_the_sender()
    {
        var sender = Guid.NewGuid();
        var bob = Guid.NewGuid();
        var carol = Guid.NewGuid();
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var chats = new FakeChatRepository { Members = [Member(sender), Member(bob), Member(carol)] };
        var notifications = new FakeNotificationRepository();
        var uow = new FakeUnitOfWork();
        var handler = new CreateNotificationsOnMessageSentHandler(chats, notifications, uow);

        await handler.Handle(
            new DomainEventNotification<MessageSentEvent>(new MessageSentEvent(messageId, chatId, sender)),
            CancellationToken.None);

        Assert.Equal(2, notifications.Added.Count);
        Assert.DoesNotContain(notifications.Added, n => n.RecipientUserId == sender);
        Assert.All(notifications.Added, n =>
        {
            Assert.Equal(NotificationType.NewMessage, n.Type);
            Assert.Equal(chatId, n.ChatId);
            Assert.Equal(messageId, n.MessageId);
        });
        Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task Does_nothing_when_the_sender_is_the_only_member()
    {
        var sender = Guid.NewGuid();
        var chats = new FakeChatRepository { Members = [Member(sender)] };
        var notifications = new FakeNotificationRepository();
        var uow = new FakeUnitOfWork();
        var handler = new CreateNotificationsOnMessageSentHandler(chats, notifications, uow);

        await handler.Handle(
            new DomainEventNotification<MessageSentEvent>(new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), sender)),
            CancellationToken.None);

        Assert.Empty(notifications.Added);
        Assert.Equal(0, uow.SaveCount);
    }
}
