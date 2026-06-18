using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Contacts.Commands.UpdateContact;

public sealed record UpdateContactCommand(
    Guid ContactUserId,
    string FirstName,
    string? LastName
) : IAppRequest;
