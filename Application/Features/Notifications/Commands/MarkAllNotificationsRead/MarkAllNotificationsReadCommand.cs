using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

public sealed record MarkAllNotificationsReadCommand() : IAppRequest;
