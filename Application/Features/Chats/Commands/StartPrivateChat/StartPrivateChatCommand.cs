using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Chats.Commands.StartPrivateChat;

/// <summary>
/// Starts (or returns the existing) one-to-one private chat between the current user and
/// <paramref name="OtherUserId"/>. Idempotent: calling it twice yields the same chat.
/// </summary>
public sealed record StartPrivateChatCommand(Guid OtherUserId) : IAppRequest<Guid>;
