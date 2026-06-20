namespace Messenger.Application.Features.Users.Presence.Dtos;

public sealed record UserPresenceDto(Guid UserId, bool IsOnline, DateTime? LastSeenAtUtc);
