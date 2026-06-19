using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Messages.Dtos;

namespace Messenger.Application.Features.Messages.Queries.GetChatMessages;

public sealed class GetChatMessagesHandler : IAppRequestHandler<GetChatMessagesQuery, List<MessageDto>>
{
    private const int MaxLimit = 100;

    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;
    private readonly IMessageRepository _messageRepository;

    public GetChatMessagesHandler(
        ICurrentUser currentUser,
        IChatRepository chatRepository,
        IMessageRepository messageRepository)
    {
        _currentUser = currentUser;
        _chatRepository = chatRepository;
        _messageRepository = messageRepository;
    }

    public async Task<Result<List<MessageDto>>> Handle(GetChatMessagesQuery request, CancellationToken ct)
    {
        if (!await _chatRepository.IsParticipantAsync(request.ChatId, _currentUser.UserId, ct))
            return Result<List<MessageDto>>.Failure(["You are not a member of this chat."]);

        var limit = Math.Clamp(request.Limit, 1, MaxLimit);

        var messages = await _messageRepository.GetChatMessagesAsync(request.ChatId, request.Before, limit, ct);

        return Result<List<MessageDto>>.Success(messages);
    }
}
