using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Auth.Queries.GetSessions
{
    public class GetSessionsHandler : IAppRequestHandler<GetSessionsQuery, List<SessionDto>>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;

        public GetSessionsHandler(ICurrentUser currentUser, IUserRepository userRepository)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        public async Task<Result<List<SessionDto>>> Handle(GetSessionsQuery request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdWithSessionsAsync(_currentUser.UserId, ct);

            if (user is null)
                return Result<List<SessionDto>>.Failure(["User not found"]);

            var sessions = user.Sessions
                .Select(x => new SessionDto(
                    x.Id,
                    x.DeviceId,
                    x.DeviceName,
                    x.DeviceType,
                    x.LastActivityAtUtc,
                    x.IsActive()))
                .ToList();

            return Result<List<SessionDto>>.Success(sessions);
        }
    }
}
