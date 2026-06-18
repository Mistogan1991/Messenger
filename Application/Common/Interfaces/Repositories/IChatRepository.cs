using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IChatRepository
{
    Task AddAsync(Chat chat, CancellationToken ct);

    Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct);
}
