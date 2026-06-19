using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Messages;

namespace Messenger.Application.Features.Messages.Commands.ReplyMessage;

public sealed class ReplyMessageHandler
: IAppRequestHandler<ReplyMessageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;

    public ReplyMessageHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMessageRepository messageRepository,
        IChatRepository chatRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
    }

    public async Task<Result<Guid>> Handle(ReplyMessageCommand request, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(request.ChatId, ct);

        if (chat is null)
            return Result<Guid>.Failure(["Chat not found"]);

        if (!chat.CanSendMessages(_currentUser.UserId))
            return Result<Guid>.Failure(["You are not allowed to send messages in this chat"]);

        var source = await _messageRepository.GetByIdAsync(request.ReplyToMessageId, ct);

        if (source is null)
            return Result<Guid>.Failure(["Message not found"]);

        var reply = Message.SendReply(
                request.ChatId,
                _currentUser.UserId,
                request.ReplyToMessageId,
                request.Content);

        await _messageRepository.AddAsync(reply, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(reply.Id);
    }
}
