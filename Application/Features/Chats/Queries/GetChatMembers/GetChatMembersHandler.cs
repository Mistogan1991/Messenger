using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Chats.Dtos;

namespace Messenger.Application.Features.Chats.Queries.GetChatMembers;

public sealed class GetChatMembersHandler : IAppRequestHandler<GetChatMembersQuery, List<ChatMemberDto>>
{
    private readonly IChatRepository _chatRepository;

    public GetChatMembersHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Result<List<ChatMemberDto>>> Handle(GetChatMembersQuery request, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(request.ChatId, ct);

        if(chat == null)
            return Result<List<ChatMemberDto>>.Failure(["Chat not found."]);

        var members = chat.Participants
            .Select(p => 
            new ChatMemberDto(
                p.UserId, 
                null, 
                null, 
                null, 
                p.Role))
            .ToList();

        return Result<List<ChatMemberDto>>.Success(members);
    }
}
