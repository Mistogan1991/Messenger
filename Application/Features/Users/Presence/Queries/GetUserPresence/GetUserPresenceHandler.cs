using Messenger.Application.Abstractions.Realtime;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Users.Presence.Dtos;

namespace Messenger.Application.Features.Users.Presence.Queries.GetUserPresence;

public sealed class GetUserPresenceHandler : IAppRequestHandler<GetUserPresenceQuery, UserPresenceDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPresenceTracker _presenceTracker;

    public GetUserPresenceHandler(IUserRepository userRepository, IPresenceTracker presenceTracker)
    {
        _userRepository = userRepository;
        _presenceTracker = presenceTracker;
    }

    public async Task<Result<UserPresenceDto>> Handle(GetUserPresenceQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
            return Result<UserPresenceDto>.Failure(["User not found."]);

        var isOnline = await _presenceTracker.IsOnlineAsync(user.Id);

        // NOTE: this does not yet honour the user's "last seen" privacy setting — tracked follow-up.
        return Result<UserPresenceDto>.Success(new UserPresenceDto(user.Id, isOnline, user.LastSeenAt));
    }
}
