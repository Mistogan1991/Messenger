using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.RemoveReaction;

public sealed record RemoveReactionCommand(
    Guid MessageId,
    string Emoji
) : IAppRequest;
