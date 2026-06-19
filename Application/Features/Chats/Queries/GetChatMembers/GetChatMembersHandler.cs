using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Chats.Dtos;

namespace Messenger.Application.Features.Chats.Queries.GetChatMembers;

public sealed class GetChatMembersHandler : IAppRequestHandler<GetChatMembersQuery, List<ChatMemberDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;

    public GetChatMembersHandler(ICurrentUser currentUser, IChatRepository chatRepository)
    {
        _currentUser = currentUser;
        _chatRepository = chatRepository;
    }

    public async Task<Result<List<ChatMemberDto>>> Handle(GetChatMembersQuery request, CancellationToken ct)
    {
        if (!await _chatRepository.IsParticipantAsync(request.ChatId, _currentUser.UserId, ct))
            return Result<List<ChatMemberDto>>.Failure(["You are not a member of this chat."]);

        var members = await _chatRepository.GetMembersAsync(request.ChatId, ct);

        return Result<List<ChatMemberDto>>.Success(members);
    }
}
