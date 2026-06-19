namespace Messenger.Application.Abstractions.Storage;

/// <summary>
/// Object storage abstraction. Implemented over MinIO/S3. The API never streams file bytes itself;
/// instead it mints short-lived pre-signed URLs so clients upload/download directly to storage.
/// </summary>
public interface IFileStorage
{
    /// <summary>Pre-signed URL the client uses to PUT the object's bytes directly to storage.</summary>
    Task<string> CreatePresignedUploadUrlAsync(
        string objectKey, string contentType, TimeSpan expiry, CancellationToken ct = default);

    /// <summary>Pre-signed URL the client uses to GET the object's bytes directly from storage.</summary>
    Task<string> CreatePresignedDownloadUrlAsync(
        string objectKey, TimeSpan expiry, CancellationToken ct = default);
}
