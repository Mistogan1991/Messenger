namespace Messenger.Application.Features.Files.Dtos;

/// <summary>
/// Everything the client needs to upload a file: the assigned <see cref="FileId"/> to later
/// attach, and the short-lived <see cref="UploadUrl"/> to PUT the bytes to.
/// </summary>
public sealed record FileUploadTicketDto(
    Guid FileId,
    string StorageKey,
    string UploadUrl,
    DateTime ExpiresAtUtc);
