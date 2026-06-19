using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Messages.Dtos;

namespace Messenger.Application.Features.Messages.Queries.GetMessage;

public sealed class GetMessageHandler : IAppRequestHandler<GetMessageQuery, MessageDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IMessageRepository _messageRepository;
    private readonly IChatRepository _chatRepository;

    public GetMessageHandler(
        ICurrentUser currentUser,
        IMessageRepository messageRepository,
        IChatRepository chatRepository)
    {
        _currentUser = currentUser;
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
    }

    public async Task<Result<MessageDto>> Handle(GetMessageQuery request, CancellationToken ct)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, ct);

        if (message is null)
            return Result<MessageDto>.Failure(["Message not found."]);

        if (!await _chatRepository.IsParticipantAsync(message.ChatId, _currentUser.UserId, ct))
            return Result<MessageDto>.Failure(["You are not a member of this chat."]);

        return Result<MessageDto>.Success(new MessageDto(
            message.Id,
            message.ChatId,
            message.SenderId,
            message.Type,
            message.Content,
            message.ReplyToMessageId,
            message.IsEdited,
            message.EditedAtUtc,
            message.CreatedAtUtc));
    }
}
