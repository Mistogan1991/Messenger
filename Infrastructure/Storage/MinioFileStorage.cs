using Messenger.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Messenger.Infrastructure.Storage;

/// <summary>MinIO/S3-backed <see cref="IFileStorage"/> that issues pre-signed upload/download URLs.</summary>
public sealed class MinioFileStorage : IFileStorage
{
    private readonly IMinioClient _client;
    private readonly MinioOptions _options;

    public MinioFileStorage(IMinioClient client, IOptions<MinioOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<string> CreatePresignedUploadUrlAsync(
        string objectKey, string contentType, TimeSpan expiry, CancellationToken ct = default)
    {
        await EnsureBucketExistsAsync(ct);

        var args = new PresignedPutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithExpiry((int)expiry.TotalSeconds);

        return await _client.PresignedPutObjectAsync(args);
    }

    public Task<string> CreatePresignedDownloadUrlAsync(
        string objectKey, TimeSpan expiry, CancellationToken ct = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithExpiry((int)expiry.TotalSeconds);

        return _client.PresignedGetObjectAsync(args);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        var exists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.Bucket), ct);

        if (!exists)
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_options.Bucket), ct);
    }
}
