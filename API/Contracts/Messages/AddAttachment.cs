namespace Messenger.API.Contracts.Messages;

public sealed record AddAttachmentRequest(
    Guid MessageId,
    Guid FileId,
    string? Caption
);
