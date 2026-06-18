using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.ValueObjects;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
    }

    public async Task<bool> ExistsUsernameAsync(Username username, CancellationToken ct = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Username != null && u.Username == username, ct);
    }

    public async Task<List<Guid>> GetExistingUserIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct)
    {
        return await _context.Users
            .Where(x => userIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, ct);
    }

    public async Task<User?> GetByIdWithContactsAsync(Guid userId, CancellationToken ct = default)
    {

        return await _context.Users
            .Include(x => x.Contacts)
            .FirstOrDefaultAsync(x => x.Id == userId, ct);
    }

    public async Task<User?> GetByIdWithBlockedUsersAsync(Guid userId, CancellationToken ct = default)
    {

        return await _context.Users
            .Include(x => x.BlockedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId, ct);
    }

    public async Task<User?> GetByIdWithSessionsAsync(Guid userId, CancellationToken ct = default)
    {

        return await _context.Users
            .Include(x => x.Sessions)
            .FirstOrDefaultAsync(x => x.Id == userId, ct);
    }

    public async Task<User?> GetByIdWithPrivacySettingsAsync(Guid userId, CancellationToken ct = default)
    {

        return await _context.Users
            .Include(x => x.PrivacySettings)
            .FirstOrDefaultAsync(x => x.Id == userId, ct);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct)
    {
        return await _context.Users
            .Include(x => x.Sessions)
            .FirstOrDefaultAsync(
                x => x.PhoneNumber == PhoneNumber.Create(phoneNumber),
                ct);
    }

    public async Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default)
    {
        return await _context.Users
            .Include(x => x.Sessions)
            .FirstOrDefaultAsync(u => u.Username != null && u.Username == username, ct);
    }

    public async Task<User?> GetBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _context.Users
            .Include(x => x.Sessions)
            .FirstOrDefaultAsync(
                x => x.Sessions.Any(s => s.Id == sessionId),
                ct);
    }
}
