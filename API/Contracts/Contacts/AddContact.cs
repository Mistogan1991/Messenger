namespace Messenger.API.Contracts.Contacts;

public sealed record AddContactRequest(
    string PhoneNumber,
    string FirstName,
    string? LastName
);
