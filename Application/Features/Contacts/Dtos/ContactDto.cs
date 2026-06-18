namespace Messenger.Application.Features.Contacts.Dtos;

public sealed record ContactDto(
    Guid UserId,
    string FirstName,
    string? LastName
);
