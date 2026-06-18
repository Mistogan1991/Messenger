using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Contacts.Commands.DeleteContact;

public sealed record DeleteContactCommand(
    Guid ContactUserId
) : IAppRequest;
