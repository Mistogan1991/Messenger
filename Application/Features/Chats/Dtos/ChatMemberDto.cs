using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Chats.Dtos;

public sealed record ChatMemberDto(
    Guid UserId,
    string? Username,
    string? FirstName,
    string? LastName,
    ChatRole Role
);