using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Messages.Commands.SendMessage;

public sealed class SendMessageHandler : IAppRequestHandler<SendMessageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;
    private readonly IMessageRepository _messageRepository;

    public SendMessageHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IChatRepository chatRepository,
        IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(request.ChatId, ct);

        if (chat is null)
            return Result<Guid>.Failure(["Chat not found"]);

        if (!chat.CanSendMessages(_currentUser.UserId))
            return Result<Guid>.Failure(["You are not allowed to send messages in this chat"]);

        var message = Message.Send(
                request.ChatId,
                _currentUser.UserId,
                MessageType.Text,
                request.Content);

        await _messageRepository.AddAsync(message, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(message.Id);
    }
}
