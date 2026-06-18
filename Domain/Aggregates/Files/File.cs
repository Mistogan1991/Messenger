using Messenger.Domain.Common;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Aggregates.Files;

public sealed class File : AggregateRoot<Guid>
{
    #region Properties

    public Guid UploadedBy { get; private set; }
    public string FileName { get; private set; }
    public string StorageKey { get; private set; }
    public string ContentType { get; private set; }
    public long Size { get; private set; }
    public FileType Type { get; private set; }
    public DateTime UploadedAtUtc { get; private set; }
    public string? ThumbnailStorageKey { get; private set; }

    #endregion

    #region Constructors

    private File() { }

    private File(
        Guid id,
        Guid uploadedBy,
        string fileName,
        string storageKey,
        string contentType,
        long size,
        FileType type): base(id)
    {
        UploadedBy = uploadedBy;
        FileName = fileName;
        StorageKey = storageKey;
        ContentType = contentType;
        Size = size;
        Type = type;
        UploadedAtUtc = DateTime.UtcNow;
    }

    #endregion

    #region Factory Method

    public static File Create(
        Guid uploadedBy,
        string fileName,
        string storageKey,
        string contentType,
        long size,
        FileType type)
    {
        var file = new File(
            Guid.NewGuid(),
            uploadedBy,
            fileName,
            storageKey,
            contentType,
            size,
            type);

        return file;
    }

    #endregion

    #region Domain Rules

    public void Delete()
    {
        MarkAsDelete();
    }

    #endregion
}
