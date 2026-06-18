namespace Messenger.API.Contracts.Messages;

public sealed record ReadMessageRequest(
    Guid ChatId,
    Guid MessageId
);
