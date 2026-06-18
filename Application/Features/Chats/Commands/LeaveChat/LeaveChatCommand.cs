using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.LeaveChat;

public sealed record LeaveChatCommand(
    Guid ChatId
) : IAppRequest;
