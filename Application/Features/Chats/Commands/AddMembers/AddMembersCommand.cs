using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.AddMembers;

public sealed record AddMembersCommand(
    Guid ChatId,
    List<Guid> UserIds
) : IAppRequest;
