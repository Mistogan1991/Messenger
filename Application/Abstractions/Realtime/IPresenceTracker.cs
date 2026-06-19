namespace Messenger.Application.Abstractions.Realtime;

/// <summary>
/// Tracks which users are currently connected (online), counting connections so a user with
/// multiple devices/tabs is only considered offline once the last one disconnects.
/// </summary>
public interface IPresenceTracker
{
    /// <summary>Records a new connection. Returns true if this is the user's first active connection.</summary>
    Task<bool> UserConnectedAsync(Guid userId);

    /// <summary>Records a dropped connection. Returns true if this was the user's last active connection.</summary>
    Task<bool> UserDisconnectedAsync(Guid userId);

    Task<bool> IsOnlineAsync(Guid userId);
}
