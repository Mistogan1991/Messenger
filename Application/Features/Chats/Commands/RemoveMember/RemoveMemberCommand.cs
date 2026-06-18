using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.RemoveMember;

public sealed record RemoveMemberCommand(
    Guid ChatId,
    Guid UserId
) : IAppRequest;
