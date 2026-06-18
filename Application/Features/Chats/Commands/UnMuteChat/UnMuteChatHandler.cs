using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Chats.Commands.UnMuteChat
{
    public sealed class UnMuteChatHandler : IAppRequestHandler<UnMuteChatCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IChatRepository _chatRepository;

        public UnMuteChatHandler(
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IChatRepository chatRepository)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _chatRepository = chatRepository;
        }

        public async Task<Result> Handle(UnMuteChatCommand request, CancellationToken ct)
        {
            var chat = await _chatRepository.GetByIdAsync(request.ChatId, ct);

            if (chat is null)
                return Result.Failure(["Chat not found."]);

            chat.UnMuteChat(_currentUser.UserId);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
