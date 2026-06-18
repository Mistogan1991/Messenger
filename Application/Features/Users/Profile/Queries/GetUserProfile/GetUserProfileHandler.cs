using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Users.Profile.Queries.GetUserProfile;

public class GetUserProfileHandler : IAppRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
            return Result<UserProfileDto>.Failure(["User not found."]);

        return Result<UserProfileDto>.Success(
            new UserProfileDto(
                user.Id,
                user.Username?.Value,
                user.FirstName,
                user.LastName,
                user.Bio));
    }
}
