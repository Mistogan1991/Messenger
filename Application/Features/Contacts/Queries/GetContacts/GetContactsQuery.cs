using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Contacts.Dtos;

namespace Messenger.Application.Features.Contacts.Queries.GetContacts;

public sealed record GetContactsQuery() : IAppRequest<List<ContactDto>>;
