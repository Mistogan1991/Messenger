using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Chats.Dtos;

namespace Messenger.Application.Features.Chats.Queries.GetMyChats;

/// <summary>Lists the chats the current user participates in, most recently active first.</summary>
public sealed record GetMyChatsQuery() : IAppRequest<List<MyChatDto>>;
