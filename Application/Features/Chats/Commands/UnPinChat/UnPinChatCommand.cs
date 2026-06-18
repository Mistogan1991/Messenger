using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.UnPinChat;

public sealed record UnPinChatCommand(
    Guid ChatId
) : IAppRequest;
