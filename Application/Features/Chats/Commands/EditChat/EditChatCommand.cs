using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.EditChat;

public sealed record EditChatCommand(
    Guid ChatId,
    string Title,
    string? Description,
    bool? IsPublic
) : IAppRequest;