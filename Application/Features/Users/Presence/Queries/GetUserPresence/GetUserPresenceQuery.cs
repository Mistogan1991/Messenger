using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Users.Presence.Dtos;

namespace Messenger.Application.Features.Users.Presence.Queries.GetUserPresence;

public sealed record GetUserPresenceQuery(Guid UserId) : IAppRequest<UserPresenceDto>;
