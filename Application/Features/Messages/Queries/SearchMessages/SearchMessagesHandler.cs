using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Messages.Dtos;

namespace Messenger.Application.Features.Messages.Queries.SearchMessages;

public sealed class SearchMessagesHandler : IAppRequestHandler<SearchMessagesQuery, List<MessageDto>>
{
    private const int MaxLimit = 100;
    private const int MinTermLength = 2;

    private readonly ICurrentUser _currentUser;
    private readonly IMessageRepository _messageRepository;

    public SearchMessagesHandler(ICurrentUser currentUser, IMessageRepository messageRepository)
    {
        _currentUser = currentUser;
        _messageRepository = messageRepository;
    }

    public async Task<Result<List<MessageDto>>> Handle(SearchMessagesQuery request, CancellationToken ct)
    {
        var term = request.Query?.Trim() ?? string.Empty;

        if (term.Length < MinTermLength)
            return Result<List<MessageDto>>.Success([]);

        var limit = Math.Clamp(request.Limit, 1, MaxLimit);

        var results = await _messageRepository.SearchAsync(_currentUser.UserId, term, limit, ct);

        return Result<List<MessageDto>>.Success(results);
    }
}
