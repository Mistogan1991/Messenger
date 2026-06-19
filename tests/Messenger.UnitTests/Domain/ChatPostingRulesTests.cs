using Messenger.Domain.Aggregates.Chats;
using Xunit;

namespace Messenger.UnitTests.Domain;

public class ChatPostingRulesTests
{
    [Fact]
    public void In_a_channel_only_owner_and_admins_can_post()
    {
        var owner = Guid.NewGuid();
        var admin = Guid.NewGuid();
        var subscriber = Guid.NewGuid();
        var channel = Chat.CreateChannel("News", owner, isPublic: true);
        channel.AddParticipant(admin);
        channel.PromoteAdmin(admin);
        channel.AddParticipant(subscriber);

        Assert.True(channel.CanSendMessages(owner));
        Assert.True(channel.CanSendMessages(admin));
        Assert.False(channel.CanSendMessages(subscriber));
    }

    [Fact]
    public void In_a_group_any_participant_can_post()
    {
        var owner = Guid.NewGuid();
        var member = Guid.NewGuid();
        var group = Chat.CreateGroup("Team", owner, new[] { member });

        Assert.True(group.CanSendMessages(owner));
        Assert.True(group.CanSendMessages(member));
    }

    [Fact]
    public void Non_participants_cannot_post()
    {
        var owner = Guid.NewGuid();
        var channel = Chat.CreateChannel("News", owner, isPublic: true);
        var group = Chat.CreateGroup("Team", owner, Array.Empty<Guid>());

        Assert.False(channel.CanSendMessages(Guid.NewGuid()));
        Assert.False(group.CanSendMessages(Guid.NewGuid()));
    }
}
