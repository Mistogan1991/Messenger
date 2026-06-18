using MediatR;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Chats.Commands.RemoveMember;

public sealed class RemoveMemberHandler : IRequestHandler<RemoveMemberCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChatRepository _chatRepository;

    public RemoveMemberHandler(
        IUnitOfWork unitOfWork,
        IChatRepository chatRepository)
    {
        _unitOfWork = unitOfWork;
        _chatRepository = chatRepository;
    }

    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken ct)
    {
        var chat = await _chatRepository.GetByIdAsync(request.ChatId, ct);

        if (chat == null)
            return Result.Failure(["Chat not found"]);

        chat.RemoveParticipant(request.UserId);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
