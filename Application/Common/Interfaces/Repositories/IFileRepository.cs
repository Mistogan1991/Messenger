using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IFileRepository
{
    Task AddAsync(File file, CancellationToken ct = default);

    Task<File?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Whether the user may access the file: they uploaded it, it is attached to a message in a
    /// chat they belong to, or it is a user profile photo.
    /// </summary>
    Task<bool> CanUserAccessAsync(Guid fileId, Guid userId, CancellationToken ct = default);
}
