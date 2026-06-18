namespace Messenger.API.Contracts.Chats;

public sealed record EditChatRequest(
    string Title,
    string? Description,
    bool? IsPublic
);
