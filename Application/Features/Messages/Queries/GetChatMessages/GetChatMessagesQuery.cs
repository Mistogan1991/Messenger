using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Messages.Dtos;

namespace Messenger.Application.Features.Messages.Queries.GetChatMessages;

/// <summary>
/// Returns a page of a chat's messages, newest first. Keyset paginated: pass the oldest
/// <c>CreatedAtUtc</c> you have as <paramref name="Before"/> to fetch the next older page.
/// </summary>
public sealed record GetChatMessagesQuery(
    Guid ChatId,
    DateTime? Before = null,
    int Limit = 50) : IAppRequest<List<MessageDto>>;
