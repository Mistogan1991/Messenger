using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.CreateChannel;

public sealed record CreateChannelCommand(
    string Title,
    string Description,
    bool IsPublic
) : IAppRequest<Guid>;
