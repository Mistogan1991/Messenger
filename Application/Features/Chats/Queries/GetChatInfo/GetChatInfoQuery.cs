using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Chats.Dtos;

namespace Messenger.Application.Features.Chats.Queries.GetChatInfo;

public sealed record GetChatInfoQuery(
    Guid ChatId
) : IAppRequest<ChatInfoDto>;
