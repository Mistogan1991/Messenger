using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Enums;
using Messenger.Domain.Events.Chats;
using Xunit;

namespace Messenger.UnitTests.Domain;

public class ChatLeaveTests
{
    private static ChatRole RoleOf(Chat chat, Guid userId) =>
        chat.Participants.Single(p => p.UserId == userId).Role;

    [Fact]
    public void Owner_leaving_transfers_ownership_to_an_admin()
    {
        var owner = Guid.NewGuid();
        var admin = Guid.NewGuid();
        var member = Guid.NewGuid();
        var chat = Chat.CreateGroup("Team", owner, new[] { admin, member });
        chat.PromoteAdmin(admin);

        chat.Leave(owner);

        Assert.DoesNotContain(chat.Participants, p => p.UserId == owner);
        Assert.Equal(ChatRole.Owner, RoleOf(chat, admin));
        Assert.Equal(ChatRole.Member, RoleOf(chat, member));
    }

    [Fact]
    public void Owner_leaving_with_no_admin_promotes_a_remaining_member()
    {
        var owner = Guid.NewGuid();
        var member = Guid.NewGuid();
        var chat = Chat.CreateGroup("Team", owner, new[] { member });

        chat.Leave(owner);

        Assert.DoesNotContain(chat.Participants, p => p.UserId == owner);
        Assert.Equal(ChatRole.Owner, RoleOf(chat, member));
    }

    [Fact]
    public void Non_owner_leaving_does_not_change_ownership()
    {
        var owner = Guid.NewGuid();
        var member = Guid.NewGuid();
        var chat = Chat.CreateGroup("Team", owner, new[] { member });

        chat.Leave(member);

        Assert.DoesNotContain(chat.Participants, p => p.UserId == member);
        Assert.Equal(ChatRole.Owner, RoleOf(chat, owner));
    }

    [Fact]
    public void Sole_owner_leaving_empties_the_chat()
    {
        var owner = Guid.NewGuid();
        var chat = Chat.CreateGroup("Team", owner, Array.Empty<Guid>());

        chat.Leave(owner);

        Assert.Empty(chat.Participants);
    }

    [Fact]
    public void Leaving_raises_a_ParticipantRemovedEvent()
    {
        var owner = Guid.NewGuid();
        var member = Guid.NewGuid();
        var chat = Chat.CreateGroup("Team", owner, new[] { member });
        chat.ClearDomainEvents();

        chat.Leave(member);

        Assert.Contains(chat.DomainEvents, e => e is ParticipantRemovedEvent);
    }

    [Fact]
    public void Leaving_when_not_a_member_is_a_no_op()
    {
        var owner = Guid.NewGuid();
        var chat = Chat.CreateGroup("Team", owner, Array.Empty<Guid>());

        chat.Leave(Guid.NewGuid());

        Assert.Single(chat.Participants);
        Assert.Equal(ChatRole.Owner, RoleOf(chat, owner));
    }
}
