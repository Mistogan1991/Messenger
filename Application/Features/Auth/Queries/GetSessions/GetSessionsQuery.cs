using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Auth.Queries.GetSessions;

public sealed record GetSessionsQuery() : IAppRequest<List<SessionDto>>;

public sealed record SessionDto(
    Guid SessionId,
    string DeviceId,
    string DeviceName,
    string DeviceType,
    DateTime LastActivityAtUtc,
    bool IsActive
);
