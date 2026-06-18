using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.AddReaction;

public sealed record AddReactionCommand(
    Guid MessageId,
    string Emoji
) : IAppRequest;
