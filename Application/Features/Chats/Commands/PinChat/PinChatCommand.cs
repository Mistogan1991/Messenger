using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.PinChat;

public sealed record PinChatCommand(
    Guid ChatId
) : IAppRequest;
