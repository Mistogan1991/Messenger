using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.UnMuteChat;

public sealed record UnMuteChatCommand(
    Guid ChatId
) : IAppRequest;
