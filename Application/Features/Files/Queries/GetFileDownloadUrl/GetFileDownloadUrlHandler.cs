using Messenger.Application.Abstractions.Storage;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Features.Files.Queries.GetFileDownloadUrl;

public sealed class GetFileDownloadUrlHandler : IAppRequestHandler<GetFileDownloadUrlQuery, string>
{
    private static readonly TimeSpan DownloadUrlLifetime = TimeSpan.FromMinutes(15);

    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public GetFileDownloadUrlHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public async Task<Result<string>> Handle(GetFileDownloadUrlQuery request, CancellationToken ct)
    {
        var file = await _fileRepository.GetByIdAsync(request.FileId, ct);

        if (file is null)
            return Result<string>.Failure(["File not found."]);

        // NOTE: any authenticated user who knows the file id can currently mint a download URL.
        // Proper access control (the file must be reachable via a chat the caller belongs to, or be
        // the caller's own profile photo) requires a cross-aggregate lookup and is a tracked follow-up.
        var url = await _fileStorage.CreatePresignedDownloadUrlAsync(file.StorageKey, DownloadUrlLifetime, ct);

        return Result<string>.Success(url);
    }
}
