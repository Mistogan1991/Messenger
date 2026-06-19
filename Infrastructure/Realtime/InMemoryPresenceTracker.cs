using System.Collections.Concurrent;
using Messenger.Application.Abstractions.Realtime;

namespace Messenger.Infrastructure.Realtime;

/// <summary>
/// Single-instance presence tracker backed by an in-process connection counter. Suitable for local
/// development and single-server deployments; use <see cref="RedisPresenceTracker"/> for scale-out.
/// </summary>
public sealed class InMemoryPresenceTracker : IPresenceTracker
{
    private readonly ConcurrentDictionary<Guid, int> _connections = new();

    public Task<bool> UserConnectedAsync(Guid userId)
    {
        var count = _connections.AddOrUpdate(userId, 1, (_, current) => current + 1);
        return Task.FromResult(count == 1);
    }

    public Task<bool> UserDisconnectedAsync(Guid userId)
    {
        var count = _connections.AddOrUpdate(userId, 0, (_, current) => current - 1);

        if (count <= 0)
            _connections.TryRemove(userId, out _);

        return Task.FromResult(count <= 0);
    }

    public Task<bool> IsOnlineAsync(Guid userId)
        => Task.FromResult(_connections.TryGetValue(userId, out var count) && count > 0);
}
