using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.DeleteMessage;

public sealed record DeleteMessageCommand(
    Guid MessageId
) : IAppRequest;