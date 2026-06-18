using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.EditMessage;

public sealed record EditMessageCommand(
    Guid MessageId,
    string Content
) : IAppRequest;
