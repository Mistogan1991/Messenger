namespace Messenger.API.Contracts.Chats;

public sealed record CreateChannelRequest(string Title, string Description, bool IsPublic);
