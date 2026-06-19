using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Application.Common.Interfaces.Repositories;

public interface IFileRepository
{
    Task AddAsync(File file, CancellationToken ct = default);

    Task<File?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
