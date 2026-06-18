using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class SoftDeleteQueryFilterTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"softdelete-{Guid.NewGuid()}")
            .Options);

    [Fact]
    public async Task Non_deleted_entities_are_returned()
    {
        await using var db = NewContext();
        db.Messages.Add(Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "alive"));
        await db.SaveChangesAsync();

        Assert.Single(await db.Messages.ToListAsync());
    }

    [Fact]
    public async Task Soft_deleted_entities_are_excluded_from_default_queries()
    {
        await using var db = NewContext();
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "doomed");
        db.Messages.Add(message);
        await db.SaveChangesAsync();

        message.Delete();
        await db.SaveChangesAsync();

        Assert.Empty(await db.Messages.ToListAsync());
    }

    [Fact]
    public async Task Soft_deleted_entities_remain_accessible_via_IgnoreQueryFilters()
    {
        await using var db = NewContext();
        var message = Message.Send(Guid.NewGuid(), Guid.NewGuid(), MessageType.Text, "doomed");
        db.Messages.Add(message);
        await db.SaveChangesAsync();

        message.Delete();
        await db.SaveChangesAsync();

        Assert.Single(await db.Messages.IgnoreQueryFilters().ToListAsync());
    }
}
