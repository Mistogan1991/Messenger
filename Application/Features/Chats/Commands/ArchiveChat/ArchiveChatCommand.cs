using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.ArchiveChat;

public sealed record ArchiveChatCommand(
    Guid ChatId
) : IAppRequest;
