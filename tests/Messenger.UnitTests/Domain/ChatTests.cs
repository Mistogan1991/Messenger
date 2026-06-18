using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Events.Chats;
using Xunit;

namespace Messenger.UnitTests.Domain;

public class ChatTests
{
    [Fact]
    public void CreateGroup_raises_ChatCreatedEvent_and_adds_owner()
    {
        var ownerId = Guid.NewGuid();

        var chat = Chat.CreateGroup("Team", ownerId, isPublic: false);

        Assert.True(chat.IsParticipant(ownerId));
        Assert.Contains(chat.DomainEvents, e => e is ChatCreatedEvent);
    }

    [Fact]
    public void IsParticipant_returns_false_for_unknown_user()
    {
        var chat = Chat.CreateGroup("Team", Guid.NewGuid(), isPublic: false);

        Assert.False(chat.IsParticipant(Guid.NewGuid()));
    }

    [Fact]
    public void AddParticipant_adds_member_and_raises_event()
    {
        var chat = Chat.CreateGroup("Team", Guid.NewGuid(), isPublic: false);
        chat.ClearDomainEvents();
        var newMember = Guid.NewGuid();

        chat.AddParticipant(newMember);

        Assert.True(chat.IsParticipant(newMember));
        Assert.Single(chat.DomainEvents, e => e is ParticipantAddedEvent);
    }

    [Fact]
    public void RemoveParticipant_removes_member_and_raises_event()
    {
        var chat = Chat.CreateGroup("Team", Guid.NewGuid(), isPublic: false);
        var member = Guid.NewGuid();
        chat.AddParticipant(member);
        chat.ClearDomainEvents();

        chat.RemoveParticipant(member);

        Assert.False(chat.IsParticipant(member));
        Assert.Single(chat.DomainEvents, e => e is ParticipantRemovedEvent);
    }
}
