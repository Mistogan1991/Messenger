using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.UnArchiveChat;

public sealed record UnArchiveChatCommand(
    Guid ChatId
) : IAppRequest;
