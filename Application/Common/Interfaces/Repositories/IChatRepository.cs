using Messenger.Application.Features.Chats.Dtos;
using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IChatRepository
{
    Task AddAsync(Chat chat, CancellationToken ct);

    Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Returns the existing private chat shared by both users, or null if none exists.</summary>
    Task<Chat?> GetPrivateChatAsync(Guid userA, Guid userB, CancellationToken ct);

    /// <summary>Projects the chats the given user participates in into a chat-list read model.</summary>
    Task<List<MyChatDto>> GetUserChatsAsync(Guid userId, CancellationToken ct);
}
