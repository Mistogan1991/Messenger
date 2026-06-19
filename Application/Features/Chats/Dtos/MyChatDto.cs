using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Chats.Dtos;

/// <summary>One entry in the current user's chat list, with their per-chat state and a last-message preview.</summary>
public sealed record MyChatDto(
    Guid ChatId,
    ChatType Type,
    string? Title,
    string? Username,
    int MembersCount,
    bool IsMuted,
    bool IsArchived,
    bool IsPinned,
    Guid? LastReadMessageId,
    string? LastMessagePreview,
    DateTime? LastMessageAtUtc,
    Guid? LastMessageSenderId);
