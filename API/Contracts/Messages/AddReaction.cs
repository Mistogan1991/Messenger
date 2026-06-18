namespace Messenger.API.Contracts.Messages;

public sealed record AddReactionRequest(
    Guid MessageId,
    string Emoji
);
