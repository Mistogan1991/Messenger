using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Users.Blocking.Commands.BlockUser;

public sealed class BlockUserHandler : IAppRequestHandler<BlockUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public BlockUserHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(BlockUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithBlockedUsersAsync(_currentUser.UserId, ct);

        if (user == null)
            return Result.Failure(["User not found."]);

        var targetUser = await _userRepository.GetByIdAsync(request.BlockedUserId, ct);

        if (targetUser is null)
            return Result.Failure(["Target user not found."]);

        user.BlockUser(request.BlockedUserId);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
