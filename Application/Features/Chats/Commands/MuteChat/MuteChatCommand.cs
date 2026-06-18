using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.MuteChat;

public sealed record MuteChatCommand(
    Guid ChatId
) : IAppRequest;
