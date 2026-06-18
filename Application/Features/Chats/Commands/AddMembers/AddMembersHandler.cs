using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Chats.Commands.AddMembers;

public sealed class AddMembersHandler : IAppRequestHandler<AddMembersCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;

    public AddMembersHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IChatRepository chatRepository,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(AddMembersCommand request, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(request.ChatId, ct);

        if (chat is null)
            return Result.Failure(["Chat not found."]);

        var existingUserIds = await _userRepository.GetExistingUserIdsAsync(request.UserIds, ct);

        chat.AddParticipants(existingUserIds);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}