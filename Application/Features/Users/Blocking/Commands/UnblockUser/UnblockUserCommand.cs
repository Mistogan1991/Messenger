using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Blocking.Commands.UnblockUser;

public sealed record UnblockUserCommand(
    Guid UserId
) : IAppRequest;
