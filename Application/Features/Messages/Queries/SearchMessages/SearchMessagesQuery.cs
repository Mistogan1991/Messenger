using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Messages.Dtos;

namespace Messenger.Application.Features.Messages.Queries.SearchMessages;

/// <summary>Searches the current user's messages (across all chats they belong to) by content.</summary>
public sealed record SearchMessagesQuery(string Query, int Limit = 50) : IAppRequest<List<MessageDto>>;
