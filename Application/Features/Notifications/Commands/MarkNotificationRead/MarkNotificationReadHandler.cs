using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Notifications.Commands.MarkNotificationRead;

public sealed class MarkNotificationReadHandler : IAppRequestHandler<MarkNotificationReadCommand>
{
    private readonly ICurrentUser _currentUser;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationReadHandler(
        ICurrentUser currentUser,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, ct);

        if (notification is null || notification.RecipientUserId != _currentUser.UserId)
            return Result.Failure(["Notification not found."]);

        notification.MarkAsRead();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
