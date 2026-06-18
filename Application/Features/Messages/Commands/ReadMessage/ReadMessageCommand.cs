using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.ReadMessage;

public sealed record ReadMessageCommand(
    Guid ChatId,
    Guid MessageId
) : IAppRequest;
