using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Features.Notifications.Dtos;
using Messenger.Domain.Aggregates.Notifications;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification, CancellationToken ct = default)
    {
        await _context.Notifications.AddAsync(notification, ct);
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);
    }

    public async Task<List<NotificationDto>> GetForUserAsync(
        Guid userId, bool unreadOnly, int limit, CancellationToken ct = default)
    {
        var query = _context.Notifications.Where(n => n.RecipientUserId == userId);

        if (unreadOnly)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.CreatedAtUtc)
            .Take(limit)
            .Select(n => new NotificationDto(
                n.Id, n.Type, n.ChatId, n.MessageId, n.IsRead, n.CreatedAtUtc))
            .ToListAsync(ct);
    }

    public Task<int> CountUnreadAsync(Guid userId, CancellationToken ct = default)
    {
        return _context.Notifications.CountAsync(n => n.RecipientUserId == userId && !n.IsRead, ct);
    }

    public async Task<int> MarkAllReadAsync(Guid userId, CancellationToken ct = default)
    {
        var unread = await _context.Notifications
            .Where(n => n.RecipientUserId == userId && !n.IsRead)
            .ToListAsync(ct);

        foreach (var notification in unread)
            notification.MarkAsRead();

        return unread.Count;
    }
}
