using Messenger.Domain.Enums;

namespace Messenger.API.Contracts.Files;

public sealed record RequestFileUploadRequest(
    string FileName,
    string ContentType,
    long Size,
    FileType Type);
