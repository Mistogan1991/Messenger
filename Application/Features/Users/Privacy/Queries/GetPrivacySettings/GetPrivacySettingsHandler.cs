using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Users.Dtos;

namespace Messenger.Application.Features.Users.Privacy.Queries.GetPrivacySettings;

public sealed class GetPrivacySettingsHandler : IAppRequestHandler<GetPrivacySettingsQuery, PrivacySettingsDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public GetPrivacySettingsHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result<PrivacySettingsDto>> Handle(GetPrivacySettingsQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithPrivacySettingsAsync(_currentUser.UserId, ct);
        if (user is null)
            return Result<PrivacySettingsDto>.Failure(["User not found."]);

        if (user.PrivacySettings is null)
            return Result<PrivacySettingsDto>.Failure(["Privacy settings not found."]);

        return Result<PrivacySettingsDto>
            .Success(
                new PrivacySettingsDto(
                    user.PrivacySettings.LastSeen,
                    user.PrivacySettings.PhoneNumber,
                    user.PrivacySettings.ProfilePhoto,
                    user.PrivacySettings.Calls,
                    user.PrivacySettings.ForwardedMessages));
    }
}
