using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Chats.Dtos;

public sealed record ChatInfoDto(
    Guid Id,
    string? Title,
    string? Username,
    ChatType Type,
    int MembersCount
);
