using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IChatRepository
{
    Task AddAsync(Chat chat, CancellationToken ct);

    Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Returns the existing private chat shared by both users, or null if none exists.</summary>
    Task<Chat?> GetPrivateChatAsync(Guid userA, Guid userB, CancellationToken ct);
}
