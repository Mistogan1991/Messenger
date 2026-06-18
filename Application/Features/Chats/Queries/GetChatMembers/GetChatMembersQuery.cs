using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Chats.Dtos;

namespace Messenger.Application.Features.Chats.Queries.GetChatMembers;

public sealed record GetChatMembersQuery(
    Guid ChatId
) : IAppRequest<List<ChatMemberDto>>;
