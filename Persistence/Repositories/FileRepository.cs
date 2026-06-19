using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using File = Messenger.Domain.Aggregates.Files.File;

namespace Messenger.Persistence.Repositories;

public sealed class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _context;

    public FileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(File file, CancellationToken ct = default)
    {
        await _context.Files.AddAsync(file, ct);
    }

    public async Task<File?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Files.FirstOrDefaultAsync(f => f.Id == id, ct);
    }
}
