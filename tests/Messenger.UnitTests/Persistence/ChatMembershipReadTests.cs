using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.ValueObjects;
using Messenger.Persistence.Context;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.UnitTests.Persistence;

public class ChatMembershipReadTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"membership-{Guid.NewGuid()}")
            .Options);

    [Fact]
    public async Task IsParticipant_is_true_for_a_member_and_false_for_a_stranger()
    {
        var me = Guid.NewGuid();
        var other = Guid.NewGuid();
        await using var db = NewContext();
        var chat = Chat.CreateGroup("Team", me, new[] { other });
        db.Chats.Add(chat);
        await db.SaveChangesAsync();

        var repo = new ChatRepository(db);

        Assert.True(await repo.IsParticipantAsync(chat.Id, me, CancellationToken.None));
        Assert.True(await repo.IsParticipantAsync(chat.Id, other, CancellationToken.None));
        Assert.False(await repo.IsParticipantAsync(chat.Id, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetMembers_returns_participants_joined_with_user_names()
    {
        await using var db = NewContext();
        var alice = User.Create(PhoneNumber.Create("+12345678901"));
        alice.CompleteProfile("Alice", null);
        var bob = User.Create(PhoneNumber.Create("+12345678902"));
        bob.CompleteProfile("Bob", "Jones");
        db.Users.AddRange(alice, bob);

        var chat = Chat.CreateGroup("Team", alice.Id, new[] { bob.Id });
        db.Chats.Add(chat);
        await db.SaveChangesAsync();

        var members = await new ChatRepository(db).GetMembersAsync(chat.Id, CancellationToken.None);

        Assert.Equal(2, members.Count);
        Assert.Contains(members, m => m.UserId == alice.Id && m.FirstName == "Alice");
        Assert.Contains(members, m => m.UserId == bob.Id && m.FirstName == "Bob" && m.LastName == "Jones");
    }
}
