namespace Messenger.API.Contracts.Messages;

public sealed record ReplyMessageRequest(
    Guid ChatId,
    Guid ReplyToMessageId,
    string Content
);
