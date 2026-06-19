using Messenger.Application.Features.Messages.Dtos;
using Messenger.Domain.Aggregates.Messages;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct = default);

    Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Returns up to <paramref name="limit"/> of a chat's messages, newest first, older than
    /// <paramref name="before"/> when provided (keyset pagination on <c>CreatedAtUtc</c>).
    /// </summary>
    Task<List<MessageDto>> GetChatMessagesAsync(Guid chatId, DateTime? before, int limit, CancellationToken ct = default);
}
