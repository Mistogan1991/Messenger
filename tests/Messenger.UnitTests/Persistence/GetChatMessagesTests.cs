using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class GetChatMessagesTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"messages-{Guid.NewGuid()}")
            .Options);

    private static async Task<Message> AddMessageAsync(ApplicationDbContext db, Guid chatId, Guid sender, string text)
    {
        var message = Message.Send(chatId, sender, MessageType.Text, text);
        db.Messages.Add(message);
        await db.SaveChangesAsync();
        await Task.Delay(3); // keep CreatedAtUtc strictly increasing for deterministic ordering
        return message;
    }

    [Fact]
    public async Task Returns_messages_newest_first_respecting_limit()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var chat = Chat.CreateGroup("c", me, Array.Empty<Guid>());
        db.Chats.Add(chat);
        await db.SaveChangesAsync();

        await AddMessageAsync(db, chat.Id, me, "one");
        await AddMessageAsync(db, chat.Id, me, "two");
        await AddMessageAsync(db, chat.Id, me, "three");

        var repo = new MessageRepository(db);

        var all = await repo.GetChatMessagesAsync(chat.Id, null, 50);
        Assert.Equal(new[] { "three", "two", "one" }, all.Select(m => m.Content));

        var limited = await repo.GetChatMessagesAsync(chat.Id, null, 2);
        Assert.Equal(new[] { "three", "two" }, limited.Select(m => m.Content));
    }

    [Fact]
    public async Task Before_returns_only_older_messages()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var chat = Chat.CreateGroup("c", me, Array.Empty<Guid>());
        db.Chats.Add(chat);
        await db.SaveChangesAsync();

        await AddMessageAsync(db, chat.Id, me, "one");
        var second = await AddMessageAsync(db, chat.Id, me, "two");
        await AddMessageAsync(db, chat.Id, me, "three");

        var older = await new MessageRepository(db).GetChatMessagesAsync(chat.Id, second.CreatedAtUtc, 50);

        Assert.Equal("one", Assert.Single(older).Content);
    }

    [Fact]
    public async Task Excludes_soft_deleted_messages()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var chat = Chat.CreateGroup("c", me, Array.Empty<Guid>());
        db.Chats.Add(chat);
        var message = Message.Send(chat.Id, me, MessageType.Text, "doomed");
        db.Messages.Add(message);
        await db.SaveChangesAsync();

        message.Delete();
        await db.SaveChangesAsync();

        Assert.Empty(await new MessageRepository(db).GetChatMessagesAsync(chat.Id, null, 50));
    }
}
