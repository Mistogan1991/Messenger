using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Messages.Commands.RemoveReaction;

public sealed class RemoveReactionHandler : IAppRequestHandler<RemoveReactionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageRepository _messageRepository;

    public RemoveReactionHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMessageRepository messageRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _messageRepository = messageRepository;
    }

    public async Task<Result> Handle(RemoveReactionCommand request, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, ct);

        if (message is null)
            return Result.Failure(["Message not found"]);

        message.RemoveReaction(
            _currentUser.UserId,
            request.Emoji);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

