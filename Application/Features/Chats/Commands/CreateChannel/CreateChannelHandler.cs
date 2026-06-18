using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Chats;

namespace Messenger.Application.Features.Chats.Commands.CreateChannel;

public sealed class CreateChannelHandler : IAppRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IChatRepository _chatRepository;

    public CreateChannelHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IChatRepository chatRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _chatRepository = chatRepository;
    }

    public async Task<Result<Guid>> Handle(CreateChannelCommand request, CancellationToken ct)
    {
        var channel = Chat.CreateChannel(request.Title, _currentUser.UserId, request.IsPublic);

        await _chatRepository.AddAsync(channel, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(channel.Id);
    }
}
