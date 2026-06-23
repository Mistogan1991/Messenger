using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Features.Chats.Commands.GetOrCreateSavedMessages;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class SavedMessagesTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"saved-{Guid.NewGuid()}")
            .Options);

    private sealed class FakeUnitOfWork(ApplicationDbContext db) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    }

    private sealed class FakeCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId { get; } = userId;
        public Guid SessionId => Guid.Empty;
        public string PhoneNumber => "+12345678901";
        public bool IsAuthenticated => true;
    }

    [Fact]
    public async Task GetSavedMessagesChat_returns_null_then_the_created_chat()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var repo = new ChatRepository(db);

        Assert.Null(await repo.GetSavedMessagesChatAsync(me, CancellationToken.None));

        db.Chats.Add(Chat.CreateSavedMessages(me));
        await db.SaveChangesAsync();

        var found = await repo.GetSavedMessagesChatAsync(me, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(ChatType.SavedMessages, found!.Type);
    }

    [Fact]
    public async Task Handler_creates_once_then_returns_the_same_chat()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var repo = new ChatRepository(db);
        var handler = new GetOrCreateSavedMessagesHandler(new FakeUnitOfWork(db), new FakeCurrentUser(me), repo);

        var first = await handler.Handle(new GetOrCreateSavedMessagesCommand(), CancellationToken.None);
        var second = await handler.Handle(new GetOrCreateSavedMessagesCommand(), CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.Equal(first.Data, second.Data);
        Assert.Single(await db.Chats.Where(c => c.Type == ChatType.SavedMessages).ToListAsync());
    }
}
