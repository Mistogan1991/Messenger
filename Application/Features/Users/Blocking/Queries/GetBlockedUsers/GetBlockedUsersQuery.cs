using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Users.Dtos;

namespace Messenger.Application.Features.Users.Blocking.Queries.GetBlockedUsers;

public sealed record GetBlockedUsersQuery() : IAppRequest<List<BlockedUserDto>>;
