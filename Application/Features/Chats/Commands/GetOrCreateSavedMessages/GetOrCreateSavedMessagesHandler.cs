using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Features.Chats.Commands.GetOrCreateSavedMessages;

public sealed class GetOrCreateSavedMessagesHandler : IAppRequestHandler<GetOrCreateSavedMessagesCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;

    public GetOrCreateSavedMessagesHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IChatRepository chatRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _chatRepository = chatRepository;
    }

    public async Task<Result<Guid>> Handle(GetOrCreateSavedMessagesCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var existing = await _chatRepository.GetSavedMessagesChatAsync(userId, ct);

        if (existing is not null)
            return Result<Guid>.Success(existing.Id);

        var chat = Chat.CreateSavedMessages(userId);

        await _chatRepository.AddAsync(chat, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(chat.Id);
    }
}
