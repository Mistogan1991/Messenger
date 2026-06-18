using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Contacts.Dtos;


namespace Messenger.Application.Features.Contacts.Queries.SearchContacts;

public sealed record SearchContactsQuery(
    string Term
) : IAppRequest<List<ContactDto>>;
