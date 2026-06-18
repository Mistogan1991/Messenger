using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Features.Chats.Commands.CreateGroup;

public sealed class CreateGroupHandler : IAppRequestHandler<CreateGroupCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;

    public CreateGroupHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IChatRepository chatRepository,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(CreateGroupCommand request, CancellationToken ct)
    {
        var members = await _userRepository.GetExistingUserIdsAsync(request.MemberIds, ct);

        var group = Chat.CreateGroup(request.Title, _currentUser.UserId, members);

        await _chatRepository.AddAsync(group, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(group.Id);
    }
}
