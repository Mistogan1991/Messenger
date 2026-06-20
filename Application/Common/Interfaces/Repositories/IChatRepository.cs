using Messenger.Application.Features.Chats.Dtos;
using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IChatRepository
{
    Task AddAsync(Chat chat, CancellationToken ct);

    Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Returns the existing private chat shared by both users, or null if none exists.</summary>
    Task<Chat?> GetPrivateChatAsync(Guid userA, Guid userB, CancellationToken ct);

    /// <summary>Returns the user's Saved Messages chat, or null if they don't have one yet.</summary>
    Task<Chat?> GetSavedMessagesChatAsync(Guid userId, CancellationToken ct);

    /// <summary>Projects the chats the given user participates in into a chat-list read model.</summary>
    Task<List<MyChatDto>> GetUserChatsAsync(Guid userId, CancellationToken ct);

    /// <summary>Returns whether the user is a (non-deleted) participant of the chat.</summary>
    Task<bool> IsParticipantAsync(Guid chatId, Guid userId, CancellationToken ct);

    /// <summary>Projects a chat's participants into member read models.</summary>
    Task<List<ChatMemberDto>> GetMembersAsync(Guid chatId, CancellationToken ct);
}
