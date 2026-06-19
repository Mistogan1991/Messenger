using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Users.Presence.Commands.MarkUserOffline;

public sealed class MarkUserOfflineHandler : IAppRequestHandler<MarkUserOfflineCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkUserOfflineHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkUserOfflineCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
            return Result.Success();

        user.SetLastSeen();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
