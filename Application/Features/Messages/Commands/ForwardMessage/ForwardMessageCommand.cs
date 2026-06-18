using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.ForwardMessage;

public sealed record ForwardMessageCommand(
    Guid TargetChatId,
    Guid SourceMessageId
) : IAppRequest<Guid>;
