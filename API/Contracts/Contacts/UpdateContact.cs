namespace Messenger.API.Contracts.Contacts;

public sealed record UpdateContactRequest(
    Guid ContactUserId,
    string FirstName,
    string? LastName
);
