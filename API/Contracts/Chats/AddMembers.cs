namespace Messenger.API.Contracts.Chats;

public sealed record AddMembersRequest(
    List<Guid> UserIds
);
