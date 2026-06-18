using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Users.Profile.Queries.GetMyProfile;

public class GetMyProfileHandler : IAppRequestHandler<GetMyProfileQuery, MyProfileDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public GetMyProfileHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result<MyProfileDto>> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, ct);

        if (user is null)
            return Result<MyProfileDto>.Failure(["User not found."]);

        return Result<MyProfileDto>.Success(
            new MyProfileDto(
                user.Id,
                user.PhoneNumber.Value,
                user.Username?.Value,
                user.FirstName,
                user.LastName,
                user.Bio));
    }
}
