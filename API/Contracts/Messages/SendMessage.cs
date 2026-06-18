namespace Messenger.API.Contracts.Messages;

public sealed record SendMessageRequest(
    Guid ChatId,
    string Content
);
