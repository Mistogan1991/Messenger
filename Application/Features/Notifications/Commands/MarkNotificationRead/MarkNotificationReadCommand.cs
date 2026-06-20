using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Notifications.Commands.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(Guid NotificationId) : IAppRequest;
