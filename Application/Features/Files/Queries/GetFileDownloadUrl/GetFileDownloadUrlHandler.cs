using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Storage;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Files.Queries.GetFileDownloadUrl;

public sealed class GetFileDownloadUrlHandler : IAppRequestHandler<GetFileDownloadUrlQuery, string>
{
    private static readonly TimeSpan DownloadUrlLifetime = TimeSpan.FromMinutes(15);

    private readonly ICurrentUser _currentUser;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public GetFileDownloadUrlHandler(
        ICurrentUser currentUser,
        IFileRepository fileRepository,
        IFileStorage fileStorage)
    {
        _currentUser = currentUser;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public async Task<Result<string>> Handle(GetFileDownloadUrlQuery request, CancellationToken ct)
    {
        var file = await _fileRepository.GetByIdAsync(request.FileId, ct);

        if (file is null)
            return Result<string>.Failure(["File not found."]);

        if (!await _fileRepository.CanUserAccessAsync(file.Id, _currentUser.UserId, ct))
            return Result<string>.Failure(["You do not have access to this file."]);

        var url = await _fileStorage.CreatePresignedDownloadUrlAsync(file.StorageKey, DownloadUrlLifetime, ct);

        return Result<string>.Success(url);
    }
}
