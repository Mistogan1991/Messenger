using Messenger.Infrastructure.Realtime;
using Xunit;

namespace Messenger.UnitTests.Infrastructure;

public class InMemoryPresenceTrackerTests
{
    [Fact]
    public async Task First_connection_reports_online_and_last_disconnection_reports_offline()
    {
        var tracker = new InMemoryPresenceTracker();
        var user = Guid.NewGuid();

        Assert.True(await tracker.UserConnectedAsync(user));   // first connection
        Assert.False(await tracker.UserConnectedAsync(user));  // second device
        Assert.True(await tracker.IsOnlineAsync(user));

        Assert.False(await tracker.UserDisconnectedAsync(user)); // one connection remains
        Assert.True(await tracker.IsOnlineAsync(user));

        Assert.True(await tracker.UserDisconnectedAsync(user));  // last connection gone
        Assert.False(await tracker.IsOnlineAsync(user));
    }

    [Fact]
    public async Task Unknown_user_is_offline()
    {
        var tracker = new InMemoryPresenceTracker();
        Assert.False(await tracker.IsOnlineAsync(Guid.NewGuid()));
    }
}
