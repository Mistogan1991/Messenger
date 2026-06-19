using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Storage;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Features.Files.Commands.RequestFileUpload;
using Messenger.Application.Features.Files.Queries.GetFileDownloadUrl;
using Messenger.Domain.Enums;
using Xunit;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.UnitTests.Application;

public class FileUploadHandlerTests
{
    private sealed class FakeCurrentUser : ICurrentUser
    {
        public Guid UserId { get; init; } = Guid.NewGuid();
        public Guid SessionId => Guid.Empty;
        public string PhoneNumber => "+12345678901";
        public bool IsAuthenticated => true;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }
        public Task<int> SaveChangesAsync(CancellationToken ct = default) { SaveCount++; return Task.FromResult(1); }
    }

    private sealed class FakeFileRepository : IFileRepository
    {
        public File? Added { get; private set; }
        public File? ToReturn { get; set; }
        public Task AddAsync(File file, CancellationToken ct = default) { Added = file; return Task.CompletedTask; }
        public Task<File?> GetByIdAsync(Guid id, CancellationToken ct = default) => Task.FromResult(ToReturn);
    }

    private sealed class FakeFileStorage : IFileStorage
    {
        public string? UploadKey { get; private set; }
        public string? DownloadKey { get; private set; }
        public Task<string> CreatePresignedUploadUrlAsync(string objectKey, string contentType, TimeSpan expiry, CancellationToken ct = default)
        {
            UploadKey = objectKey;
            return Task.FromResult($"https://storage/upload/{objectKey}");
        }
        public Task<string> CreatePresignedDownloadUrlAsync(string objectKey, TimeSpan expiry, CancellationToken ct = default)
        {
            DownloadKey = objectKey;
            return Task.FromResult($"https://storage/download/{objectKey}");
        }
    }

    [Fact]
    public async Task RequestUpload_persists_a_file_and_returns_a_ticket_with_an_upload_url()
    {
        var user = new FakeCurrentUser();
        var repo = new FakeFileRepository();
        var storage = new FakeFileStorage();
        var uow = new FakeUnitOfWork();
        var handler = new RequestFileUploadHandler(user, repo, storage, uow);

        var result = await handler.Handle(
            new RequestFileUploadCommand("photo.png", "image/png", 1024, FileType.Image), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(repo.Added);
        Assert.Equal(repo.Added!.Id, result.Data.FileId);
        Assert.Equal(repo.Added.StorageKey, result.Data.StorageKey);
        Assert.Equal(storage.UploadKey, result.Data.StorageKey);
        Assert.Contains("upload", result.Data.UploadUrl);
        Assert.Equal(1, uow.SaveCount);
    }

    [Fact]
    public async Task GetDownloadUrl_fails_when_the_file_does_not_exist()
    {
        var handler = new GetFileDownloadUrlHandler(new FakeFileRepository { ToReturn = null }, new FakeFileStorage());

        var result = await handler.Handle(new GetFileDownloadUrlQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetDownloadUrl_returns_a_presigned_url_for_the_stored_key()
    {
        var file = File.Create(Guid.NewGuid(), "doc.pdf", "key/abc.pdf", "application/pdf", 10, FileType.Document);
        var storage = new FakeFileStorage();
        var handler = new GetFileDownloadUrlHandler(new FakeFileRepository { ToReturn = file }, storage);

        var result = await handler.Handle(new GetFileDownloadUrlQuery(file.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("key/abc.pdf", storage.DownloadKey);
        Assert.Contains("download", result.Data);
    }
}
