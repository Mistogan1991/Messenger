using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Users.Privacy.Commands.UpdatePrivacySettings;

public sealed class UpdatePrivacySettingsHandler : IAppRequestHandler<UpdatePrivacySettingsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public UpdatePrivacySettingsHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(UpdatePrivacySettingsCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithPrivacySettingsAsync(_currentUser.UserId, ct);

        if (user is null)
            return Result.Failure(["User not found."]);

        user.UpdatePrivacySettings(
            request.LastSeen,
            request.PhoneNumber,
            request.ProfilePhoto,
            request.Calls,
            request.ForwardedMessages);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
