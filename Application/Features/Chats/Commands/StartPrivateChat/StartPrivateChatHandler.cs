using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Features.Chats.Commands.StartPrivateChat;

public sealed class StartPrivateChatHandler : IAppRequestHandler<StartPrivateChatCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;

    public StartPrivateChatHandler(
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

    public async Task<Result<Guid>> Handle(StartPrivateChatCommand request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        if (request.OtherUserId == currentUserId)
            return Result<Guid>.Failure(["Cannot start a private chat with yourself."]);

        var otherUser = await _userRepository.GetByIdAsync(request.OtherUserId, ct);

        if (otherUser is null)
            return Result<Guid>.Failure(["User not found."]);

        var existing = await _chatRepository.GetPrivateChatAsync(currentUserId, request.OtherUserId, ct);

        if (existing is not null)
            return Result<Guid>.Success(existing.Id);

        var chat = Chat.CreatePrivate(currentUserId, request.OtherUserId);

        await _chatRepository.AddAsync(chat, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(chat.Id);
    }
}
