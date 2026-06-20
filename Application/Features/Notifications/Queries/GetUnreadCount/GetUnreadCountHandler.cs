using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Notifications.Queries.GetUnreadCount;

public sealed class GetUnreadCountHandler : IAppRequestHandler<GetUnreadCountQuery, int>
{
    private readonly ICurrentUser _currentUser;
    private readonly INotificationRepository _notificationRepository;

    public GetUnreadCountHandler(ICurrentUser currentUser, INotificationRepository notificationRepository)
    {
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        var count = await _notificationRepository.CountUnreadAsync(_currentUser.UserId, ct);
        return Result<int>.Success(count);
    }
}
