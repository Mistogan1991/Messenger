namespace Messenger.API.Contracts.Chats;

public sealed record CreateGroupRequest(
    string Title, 
    List<Guid> MemberIds
);
