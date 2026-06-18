using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Contacts.Dtos;

namespace Messenger.Application.Features.Contacts.Queries.GetContacts;

public sealed class GetContactsHandler : IAppRequestHandler<GetContactsQuery, List<ContactDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public GetContactsHandler(ICurrentUser currentUser, IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result<List<ContactDto>>> Handle(GetContactsQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithContactsAsync(_currentUser.UserId, ct);
        if (user == null)
            return Result<List<ContactDto>>.Failure(["User not found"]);

        return Result<List<ContactDto>>.Success(
                user.Contacts
                    .Select(x =>
                        new ContactDto(
                            x.ContactUserId,
                            x.FirstName,
                            x.LastName))
                    .ToList());
    }
}
