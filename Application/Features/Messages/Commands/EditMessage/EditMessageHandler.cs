using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Messages.Commands.EditMessage;

public sealed class EditMessageHandler : IAppRequestHandler<EditMessageCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageRepository _messageRepository;

    public EditMessageHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMessageRepository messageRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _messageRepository = messageRepository;
    }

    public async Task<Result> Handle(EditMessageCommand request, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, ct);

        if (message is null)
            return Result.Failure(["Message not found"]);

        if (message.SenderId != _currentUser.UserId)
            return Result.Failure(["Access denied"]);

        message.Edit(request.Content);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
