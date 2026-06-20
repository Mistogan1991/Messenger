using Messenger.Domain.Aggregates.Notifications;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class NotificationRepositoryTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"notifications-{Guid.NewGuid()}")
            .Options);

    private static Notification New(Guid userId) =>
        Notification.Create(userId, NotificationType.NewMessage, Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public async Task GetForUser_returns_only_the_users_notifications_newest_first()
    {
        var me = Guid.NewGuid();
        var other = Guid.NewGuid();
        await using var db = NewContext();
        db.Notifications.AddRange(New(me), New(me), New(other));
        await db.SaveChangesAsync();

        var mine = await new NotificationRepository(db).GetForUserAsync(me, unreadOnly: false, limit: 50);

        Assert.Equal(2, mine.Count);
        Assert.All(mine, n => Assert.False(n.IsRead));
    }

    [Fact]
    public async Task UnreadOnly_and_CountUnread_reflect_read_state()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        var a = New(me);
        var b = New(me);
        db.Notifications.AddRange(a, b);
        await db.SaveChangesAsync();

        var repo = new NotificationRepository(db);
        Assert.Equal(2, await repo.CountUnreadAsync(me));

        a.MarkAsRead();
        await db.SaveChangesAsync();

        Assert.Equal(1, await repo.CountUnreadAsync(me));
        Assert.Single(await repo.GetForUserAsync(me, unreadOnly: true, limit: 50));
    }

    [Fact]
    public async Task MarkAllRead_marks_every_unread_notification()
    {
        var me = Guid.NewGuid();
        await using var db = NewContext();
        db.Notifications.AddRange(New(me), New(me), New(me));
        await db.SaveChangesAsync();

        var repo = new NotificationRepository(db);
        var affected = await repo.MarkAllReadAsync(me);
        await db.SaveChangesAsync();

        Assert.Equal(3, affected);
        Assert.Equal(0, await repo.CountUnreadAsync(me));
    }
}
