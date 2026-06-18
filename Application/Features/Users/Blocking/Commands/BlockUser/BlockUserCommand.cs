using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Blocking.Commands.BlockUser;

public sealed record BlockUserCommand(
    Guid BlockedUserId
) : IAppRequest;
