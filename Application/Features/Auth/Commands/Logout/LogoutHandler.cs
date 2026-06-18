using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Auth.Commands.Logout;

public sealed class LogoutHandler : IAppRequestHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public LogoutHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithSessionsAsync(_currentUser.UserId, ct);

        if (user is null)
            return Result.Failure(["User not found"]);

        user.RevokeSession(_currentUser.SessionId);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}