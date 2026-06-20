using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Notifications.Dtos;

namespace Messenger.Application.Features.Notifications.Queries.GetMyNotifications;

public sealed record GetMyNotificationsQuery(bool UnreadOnly = false, int Limit = 50)
    : IAppRequest<List<NotificationDto>>;
