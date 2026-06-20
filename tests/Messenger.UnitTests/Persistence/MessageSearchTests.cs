using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class MessageSearchTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"search-{Guid.NewGuid()}")
            .Options);

    [Fact]
    public async Task Finds_matching_messages_in_the_users_chats_case_insensitively()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var chat = Chat.CreateGroup("Team", me, Array.Empty<Guid>());
        db.Chats.Add(chat);
        db.Messages.Add(Message.Send(chat.Id, me, MessageType.Text, "Let's deploy on Friday"));
        db.Messages.Add(Message.Send(chat.Id, me, MessageType.Text, "unrelated chatter"));
        await db.SaveChangesAsync();

        var results = await new MessageRepository(db).SearchAsync(me, "DEPLOY", 50, CancellationToken.None);

        var dto = Assert.Single(results);
        Assert.Contains("deploy", dto.Content!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Does_not_return_messages_from_chats_the_user_is_not_in()
    {
        var me = Guid.NewGuid();
        var other = Guid.NewGuid();
        await using var db = NewContext();
        var otherChat = Chat.CreateGroup("Secret", other, Array.Empty<Guid>());
        db.Chats.Add(otherChat);
        db.Messages.Add(Message.Send(otherChat.Id, other, MessageType.Text, "deploy secrets"));
        await db.SaveChangesAsync();

        var results = await new MessageRepository(db).SearchAsync(me, "deploy", 50, CancellationToken.None);

        Assert.Empty(results);
    }

    [Fact]
    public async Task Excludes_soft_deleted_messages()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var chat = Chat.CreateGroup("Team", me, Array.Empty<Guid>());
        db.Chats.Add(chat);
        var message = Message.Send(chat.Id, me, MessageType.Text, "deploy me");
        db.Messages.Add(message);
        await db.SaveChangesAsync();

        message.Delete();
        await db.SaveChangesAsync();

        Assert.Empty(await new MessageRepository(db).SearchAsync(me, "deploy", 50, CancellationToken.None));
    }
}
