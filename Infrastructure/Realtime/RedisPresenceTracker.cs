using Messenger.Application.Abstractions.Realtime;
using StackExchange.Redis;

namespace Messenger.Infrastructure.Realtime;

/// <summary>
/// Presence tracker backed by Redis so online state is shared across API instances. A per-user
/// counter is incremented/decremented on connect/disconnect; the user is online while it is &gt; 0.
/// </summary>
public sealed class RedisPresenceTracker : IPresenceTracker
{
    private readonly IConnectionMultiplexer _redis;

    public RedisPresenceTracker(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    private static RedisKey Key(Guid userId) => $"presence:{userId:N}";

    public async Task<bool> UserConnectedAsync(Guid userId)
    {
        var count = await _redis.GetDatabase().StringIncrementAsync(Key(userId));
        return count == 1;
    }

    public async Task<bool> UserDisconnectedAsync(Guid userId)
    {
        var db = _redis.GetDatabase();
        var count = await db.StringDecrementAsync(Key(userId));

        if (count <= 0)
            await db.KeyDeleteAsync(Key(userId));

        return count <= 0;
    }

    public async Task<bool> IsOnlineAsync(Guid userId)
    {
        var value = await _redis.GetDatabase().StringGetAsync(Key(userId));
        return value.HasValue && (long)value > 0;
    }
}
