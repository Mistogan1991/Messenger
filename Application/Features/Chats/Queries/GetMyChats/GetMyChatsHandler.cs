using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Chats.Dtos;

namespace Messenger.Application.Features.Chats.Queries.GetMyChats;

public sealed class GetMyChatsHandler : IAppRequestHandler<GetMyChatsQuery, List<MyChatDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;

    public GetMyChatsHandler(ICurrentUser currentUser, IChatRepository chatRepository)
    {
        _currentUser = currentUser;
        _chatRepository = chatRepository;
    }

    public async Task<Result<List<MyChatDto>>> Handle(GetMyChatsQuery request, CancellationToken ct)
    {
        var chats = await _chatRepository.GetUserChatsAsync(_currentUser.UserId, ct);

        return Result<List<MyChatDto>>.Success(chats);
    }
}
