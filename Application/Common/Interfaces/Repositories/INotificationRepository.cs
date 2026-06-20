using Messenger.Application.Features.Notifications.Dtos;
using Messenger.Domain.Aggregates.Notifications;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct = default);

    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Recent notifications for a user, newest first; optionally only unread ones.</summary>
    Task<List<NotificationDto>> GetForUserAsync(Guid userId, bool unreadOnly, int limit, CancellationToken ct = default);

    Task<int> CountUnreadAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Marks all of the user's unread notifications as read; returns the number affected.</summary>
    Task<int> MarkAllReadAsync(Guid userId, CancellationToken ct = default);
}
