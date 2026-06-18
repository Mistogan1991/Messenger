using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.CreateGroup;

public sealed record CreateGroupCommand(
    string Title,
    List<Guid> MemberIds
) : IAppRequest<Guid>;
