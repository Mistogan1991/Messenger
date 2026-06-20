using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Notifications.Queries.GetUnreadCount;

public sealed record GetUnreadCountQuery() : IAppRequest<int>;
