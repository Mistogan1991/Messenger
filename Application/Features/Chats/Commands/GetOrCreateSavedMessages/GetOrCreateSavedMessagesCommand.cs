using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.GetOrCreateSavedMessages;

/// <summary>
/// Returns the current user's "Saved Messages" chat, creating it on first use. Idempotent:
/// every user has exactly one Saved Messages chat.
/// </summary>
public sealed record GetOrCreateSavedMessagesCommand() : IAppRequest<Guid>;
