using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Notifications.Dtos;

public sealed record NotificationDto(
    Guid Id,
    NotificationType Type,
    Guid? ChatId,
    Guid? MessageId,
    bool IsRead,
    DateTime CreatedAtUtc);
