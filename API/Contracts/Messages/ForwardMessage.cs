namespace Messenger.API.Contracts.Messages;

public sealed record ForwardMessageRequest(
    Guid TargetChatId,
    Guid SourceMessageId
);
