using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.ReplyMessage;

public sealed record ReplyMessageCommand(
    Guid ChatId,
    Guid ReplyToMessageId,
    string Content
) : IAppRequest<Guid>;
