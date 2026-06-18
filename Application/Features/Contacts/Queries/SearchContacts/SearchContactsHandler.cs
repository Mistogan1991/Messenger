using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Contacts.Dtos;

namespace Messenger.Application.Features.Contacts.Queries.SearchContacts
{
    public sealed class SearchContactsHandler : IAppRequestHandler<SearchContactsQuery, List<ContactDto>>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;

        public SearchContactsHandler(ICurrentUser currentUser, IUserRepository userRepository)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        public async Task<Result<List<ContactDto>>> Handle(SearchContactsQuery request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdWithContactsAsync(_currentUser.UserId, ct);

            if (user == null)
                return Result<List<ContactDto>>.Failure(["User not found."]);

            var result = string.IsNullOrWhiteSpace(request.Term)
                ? user.Contacts.ToList()
                : user.Contacts.Where(c =>
                    c.FirstName.Contains(request.Term, StringComparison.OrdinalIgnoreCase) ||
                    (c.LastName != null && c.LastName.Contains(request.Term, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return Result<List<ContactDto>>.Success(
                    result
                    .Select(x =>
                        new ContactDto(
                            x.ContactUserId,
                            x.FirstName,
                            x.LastName))
                    .ToList());
        }
    }
}
