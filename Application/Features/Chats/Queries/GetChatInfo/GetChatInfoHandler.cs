using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Chats.Dtos;

namespace Messenger.Application.Features.Chats.Queries.GetChatInfo;

public sealed class GetChatInfoHandler : IAppRequestHandler<GetChatInfoQuery, ChatInfoDto>
{
    private readonly IChatRepository _chatRepository;

    public GetChatInfoHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Result<ChatInfoDto>> Handle(GetChatInfoQuery request, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(request.ChatId, ct);

        if (chat == null)
            return Result<ChatInfoDto>.Failure(["Chat not found"]);

        return Result<ChatInfoDto>.Success(
            new ChatInfoDto(
                chat.Id,
                chat.Title,
                chat.Username,
                chat.Type,
                chat.Participants.Count));
    }
}
