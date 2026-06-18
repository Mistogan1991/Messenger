using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.PromoteAdmin;

public sealed record PromoteAdminCommand(
    Guid ChatId,
    Guid UserId
) : IAppRequest;
