using Messenger.Domain.Aggregates.Messages;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct = default);

    Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default);

}
