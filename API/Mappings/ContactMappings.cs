using Messenger.API.Contracts.Contacts;
using Messenger.Application.Features.Contacts.Commands.AddContact;
using Messenger.Application.Features.Contacts.Commands.UpdateContact;

namespace Messenger.API.Mappings;

public static class ContactMappings
{
    public static AddContactCommand ToCommand(this AddContactRequest request)
    {
        return new AddContactCommand(
            request.PhoneNumber,
            request.FirstName,
            request.LastName);
    }

    public static UpdateContactCommand ToCommand(this UpdateContactRequest request)
    {
        return new UpdateContactCommand(
            request.ContactUserId,
            request.FirstName,
            request.LastName);
    }
}