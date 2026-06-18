using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Contacts.Commands.AddContact;

public sealed class AddContactHandler : IAppRequestHandler<AddContactCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public AddContactHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(AddContactCommand request, CancellationToken ct)
    {
        var owner = await _userRepository.GetByIdAsync(_currentUser.UserId, ct);

        if (owner is null)
            return Result.Failure(["User not found."]);

        var contactUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, ct);
        
        if (contactUser is null)
            return Result.Failure(["User with this phone number is not registered."]);

        owner.AddContact(
            contactUser.Id,
            request.FirstName,
            request.LastName);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
