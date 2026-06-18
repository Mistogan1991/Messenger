using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.ValueObjects;

namespace Messenger.Application.Features.Users.Profile.Commands.SetUsername;

public class SetUsernameHandler : IAppRequestHandler<SetUsernameCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public SetUsernameHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(SetUsernameCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return Result.Failure(["Username is required."]);

        var normalizedUsername = Username.Create(request.Username);

        var exists = await _userRepository.GetByUsernameAsync(normalizedUsername, ct);

        if (exists is not null && exists.Id != _currentUser.UserId)
            return Result.Failure(["Username already taken."]);

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, ct);

        if (user == null)
            return Result.Failure(["User not found"]);

        user.SetUsername(Username.Create(request.Username));

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
