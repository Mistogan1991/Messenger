using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Users.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileHandler : IAppRequestHandler<UpdateMyProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public UpdateMyProfileHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(UpdateMyProfileCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, ct);

        if (user == null)
            return Result.Failure(["User not Found."]);

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Bio);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
