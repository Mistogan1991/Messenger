using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Messages.Dtos;

public sealed record MessageDto(
    Guid Id,
    Guid ChatId,
    Guid SenderId,
    MessageType Type,
    string? Content,
    Guid? ReplyToMessageId,
    bool IsEdited,
    DateTime? EditedAtUtc,
    DateTime CreatedAtUtc);
