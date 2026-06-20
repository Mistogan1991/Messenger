using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Notifications.Dtos;

namespace Messenger.Application.Features.Notifications.Queries.GetMyNotifications;

public sealed class GetMyNotificationsHandler : IAppRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
{
    private const int MaxLimit = 100;

    private readonly ICurrentUser _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public GetMyNotificationsHandler(ICurrentUser currentUser, INotificationRepository notificationRepository)
    {
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<List<NotificationDto>>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var limit = Math.Clamp(request.Limit, 1, MaxLimit);

        var notifications = await _notificationRepository.GetForUserAsync(
            _currentUser.UserId, request.UnreadOnly, limit, ct);

        return Result<List<NotificationDto>>.Success(notifications);
    }
}
