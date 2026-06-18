using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.SendMessage;

public sealed record SendMessageCommand(
    Guid ChatId,
    string Content
) : IAppRequest<Guid>;
