using Messenger.Domain.Common;

namespace Messenger.Domain.Aggregates.Users;

public sealed class UserSession : SoftDeletableEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string DeviceId { get; private set; }
    public string DeviceName { get; private set; }
    public string DeviceType { get; private set; }
    public string RefreshTokenHash { get; private set; }
    public DateTime RefreshTokenExpiresAtUtc { get; private set; }
    public DateTime LastActivityAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    private UserSession() { }

    private UserSession(
        Guid id,
        Guid userId,
        string deviceId,
        string deviceName,
        string deviceType,
        string refreshTokenHash,
        DateTime refreshTokenExpiresAtUtc) : base(id)
    {
        UserId = userId;
        DeviceId = deviceId;
        DeviceName = deviceName;
        DeviceType = deviceType;
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;
        LastActivityAtUtc = DateTime.UtcNow;
    }

    public static UserSession Create(
        Guid userId,
        string deviceId,
        string deviceName,
        string deviceType,
        string refreshTokenHash,
        DateTime refreshTokenExpiresAtUtc)
    {
        return new UserSession(
            Guid.NewGuid(),
            userId,
            deviceId,
            deviceName,
            deviceType,
            refreshTokenHash,
            refreshTokenExpiresAtUtc);
    }

    public void UpdateActivity()
    {
        LastActivityAtUtc = DateTime.UtcNow;
        SetUpdated(UserId);
    }

    public void Revoke()
    {
        if (RevokedAtUtc is not null) return;

        RevokedAtUtc = DateTime.UtcNow;
        SetUpdated(UserId);
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= RefreshTokenExpiresAtUtc;
    }

    public bool IsActive()
    {
        return RevokedAtUtc is null && !IsExpired();
    }

    public void ReplaceRefreshToken(string refreshTokenHash, DateTime expiresAtUtc)
    {
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAtUtc = expiresAtUtc;
        LastActivityAtUtc = DateTime.UtcNow;
        SetUpdated(UserId);
    }
}