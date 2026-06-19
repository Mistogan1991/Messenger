using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Storage;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Files.Dtos;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Application.Features.Files.Commands.RequestFileUpload;

public sealed class RequestFileUploadHandler : IAppRequestHandler<RequestFileUploadCommand, FileUploadTicketDto>
{
    private static readonly TimeSpan UploadUrlLifetime = TimeSpan.FromMinutes(15);

    private readonly ICurrentUser _currentUser;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;

    public RequestFileUploadHandler(
        ICurrentUser currentUser,
        IFileRepository fileRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FileUploadTicketDto>> Handle(RequestFileUploadCommand request, CancellationToken ct)
    {
        var storageKey = BuildStorageKey(_currentUser.UserId, request.FileName);

        var file = File.Create(
            _currentUser.UserId,
            request.FileName,
            storageKey,
            request.ContentType,
            request.Size,
            request.Type);

        await _fileRepository.AddAsync(file, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var uploadUrl = await _fileStorage.CreatePresignedUploadUrlAsync(
            storageKey, request.ContentType, UploadUrlLifetime, ct);

        return Result<FileUploadTicketDto>.Success(new FileUploadTicketDto(
            file.Id,
            storageKey,
            uploadUrl,
            DateTime.UtcNow.Add(UploadUrlLifetime)));
    }

    private static string BuildStorageKey(Guid userId, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return $"{userId:N}/{Guid.NewGuid():N}{extension}";
    }
}
