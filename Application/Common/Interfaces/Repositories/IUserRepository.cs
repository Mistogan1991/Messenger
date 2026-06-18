using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.ValueObjects;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct = default);

    Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default);

    Task<User?> GetByIdWithContactsAsync(Guid userId, CancellationToken ct = default);

    Task<User?> GetByIdWithBlockedUsersAsync(Guid userId, CancellationToken ct = default);

    Task<User?> GetByIdWithSessionsAsync(Guid userId, CancellationToken ct = default);

    Task<User?> GetByIdWithPrivacySettingsAsync(Guid userId, CancellationToken ct = default);

    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

    Task<User?> GetBySessionAsync(Guid sessionId, CancellationToken ct = default);

    Task<bool> ExistsUsernameAsync(Username username, CancellationToken ct = default);

    Task<List<Guid>> GetExistingUserIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default);

    Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default);
}
