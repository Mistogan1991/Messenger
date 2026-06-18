namespace Messenger.Application.Features.Users.Dtos;

public sealed record BlockedUserDto(
    Guid UserId,
    string? Username,
    string? FirstName,
    string? LastName,
    Guid? PhotoId
);
