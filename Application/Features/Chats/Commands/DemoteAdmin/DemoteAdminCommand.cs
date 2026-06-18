using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.PromoteAdmin;

public sealed record DemoteAdminCommand(
    Guid ChatId,
    Guid UserId
) : IAppRequest;
