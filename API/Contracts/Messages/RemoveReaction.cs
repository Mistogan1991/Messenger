namespace Messenger.API.Contracts.Messages;

public sealed record RemoveReactionRequest(
    Guid MessageId,
    string Emoji
); 
