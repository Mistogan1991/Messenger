using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Messages;

namespace Messenger.Application.Features.Messages.Commands.ForwardMessage;

public sealed class ForwardMessageHandler : IAppRequestHandler<ForwardMessageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;

    public ForwardMessageHandler(
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

    public async Task<Result<Guid>> Handle(ForwardMessageCommand request, CancellationToken ct)
    {
        var targetChat = await _chatRepository.GetByIdAsync(request.TargetChatId, ct);

        if (targetChat is null)
            return Result<Guid>.Failure(["Chat not found"]);

        if (!targetChat.CanSendMessages(_currentUser.UserId))
            return Result<Guid>.Failure(["You are not allowed to send messages in this chat"]);

        var source = await _messageRepository.GetByIdAsync(request.SourceMessageId, ct);

        if (source is null)
            return Result<Guid>.Failure(["Message not found"]);

        var forwarded = Message.Forward(
                request.TargetChatId,
                _currentUser.UserId,
                source.Id,
                source.SenderId,
                source.ChatId);

        await _messageRepository.AddAsync(forwarded, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(forwarded.Id);
    }
}
