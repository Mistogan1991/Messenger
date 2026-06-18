using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Contacts.Commands.UpdateContact;

public sealed class UpdateContactHandler : IAppRequestHandler<UpdateContactCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public UpdateContactHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }
    public async Task<Result> Handle(UpdateContactCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithContactsAsync(_currentUser.UserId, ct);

        if (user == null)
            return Result.Failure(["User not found."]);

        user.UpdateContact(
            request.ContactUserId,
            request.FirstName,
            request.LastName);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}