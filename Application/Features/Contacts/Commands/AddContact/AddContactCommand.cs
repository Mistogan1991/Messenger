using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Contacts.Commands.AddContact;

public sealed record AddContactCommand(
    string PhoneNumber,
    string FirstName,
    string? LastName
) : IAppRequest;
