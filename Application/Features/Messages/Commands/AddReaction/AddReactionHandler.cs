using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Messages.Commands.AddReaction;

public sealed class AddReactionHandler : IAppRequestHandler<AddReactionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageRepository _messageRepository;

    public AddReactionHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMessageRepository messageRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _messageRepository = messageRepository;
    }

    public async Task<Result> Handle(AddReactionCommand request, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, ct);

        if (message is null)
            return Result.Failure(["Message not found"]);

        message.AddReaction(
            _currentUser.UserId,
            request.Emoji);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
