using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Files.Dtos;
using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Files.Commands.RequestFileUpload;

/// <summary>
/// Reserves a file record and returns a pre-signed URL for the client to upload the bytes to.
/// The returned <c>FileId</c> is then referenced when attaching the file to a message or profile.
/// </summary>
public sealed record RequestFileUploadCommand(
    string FileName,
    string ContentType,
    long Size,
    FileType Type) : IAppRequest<FileUploadTicketDto>;
