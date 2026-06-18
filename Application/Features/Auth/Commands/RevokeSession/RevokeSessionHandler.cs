using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Abstractions.Authentication;

namespace Messenger.Application.Features.Auth.Commands.RevokeSession
{
    internal class RevokeSessionHandler : IAppRequestHandler<RevokeSessionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;

        public RevokeSessionHandler(
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(RevokeSessionCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdWithSessionsAsync(_currentUser.UserId, ct);

            if (user is null)
                return Result.Failure(["User not found"]);

            user.RevokeSession(_currentUser.SessionId);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
