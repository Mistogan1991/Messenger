using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Users.Dtos;

namespace Messenger.Application.Features.Users.Blocking.Queries.GetBlockedUsers;

public sealed class GetBlockedUsersHandler : IAppRequestHandler<GetBlockedUsersQuery, List<BlockedUserDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public GetBlockedUsersHandler(ICurrentUser currentUser, IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result<List<BlockedUserDto>>> Handle(GetBlockedUsersQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithBlockedUsersAsync(_currentUser.UserId, ct);

        if (user == null)
            return Result<List<BlockedUserDto>>.Failure(["User not found"]);

        //TODO
        return Result<List<BlockedUserDto>>
            .Success(
                user.BlockedUsers
                    .Select(x => new BlockedUserDto(x.BlockedUserId, null, null, null, null))
                    .ToList());
    }
}
