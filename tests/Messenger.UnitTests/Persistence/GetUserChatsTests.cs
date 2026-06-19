using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class GetUserChatsTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"mychats-{Guid.NewGuid()}")
            .Options);

    [Fact]
    public async Task Returns_chats_the_user_participates_in_with_last_message_preview()
    {
        var me = Guid.NewGuid();
        var other = Guid.NewGuid();

        await using var db = NewContext();
        var chat = Chat.CreateGroup("Team", me, new[] { other });
        db.Chats.Add(chat);
        db.Messages.Add(Message.Send(chat.Id, other, MessageType.Text, "hello"));
        await db.SaveChangesAsync();

        var result = await new ChatRepository(db).GetUserChatsAsync(me, CancellationToken.None);

        var dto = Assert.Single(result);
        Assert.Equal(chat.Id, dto.ChatId);
        Assert.Equal(ChatType.Group, dto.Type);
        Assert.Equal(2, dto.MembersCount);
        Assert.Equal("hello", dto.LastMessagePreview);
        Assert.Equal(other, dto.LastMessageSenderId);
        Assert.False(dto.IsMuted);
        Assert.Null(dto.LastReadMessageId);
    }

    [Fact]
    public async Task Excludes_chats_the_user_is_not_a_member_of()
    {
        var owner = Guid.NewGuid();
        await using var db = NewContext();
        db.Chats.Add(Chat.CreateGroup("Private group", owner, Array.Empty<Guid>()));
        await db.SaveChangesAsync();

        var result = await new ChatRepository(db).GetUserChatsAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Returns_null_preview_for_a_chat_with_no_messages()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        db.Chats.Add(Chat.CreateGroup("Empty", me, Array.Empty<Guid>()));
        await db.SaveChangesAsync();

        var dto = Assert.Single(await new ChatRepository(db).GetUserChatsAsync(me, CancellationToken.None));

        Assert.Null(dto.LastMessagePreview);
        Assert.Null(dto.LastMessageAtUtc);
        Assert.Null(dto.LastMessageSenderId);
    }
}
