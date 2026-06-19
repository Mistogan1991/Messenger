using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Domain.Aggregates.Users;
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

    public async Task<bool> CanUserAccessAsync(Guid fileId, Guid userId, CancellationToken ct = default)
    {
        // Soft-delete query filters exclude deleted files/attachments/messages/chats automatically.

        // 1) The uploader always has access.
        if (await _context.Files.AnyAsync(f => f.Id == fileId && f.UploadedBy == userId, ct))
            return true;

        // 2) Attached to a message in a chat the user participates in.
        var reachableViaChat = await _context.Set<MessageAttachment>()
            .Where(a => a.FileId == fileId)
            .Join(_context.Messages, a => a.MessageId, m => m.Id, (a, m) => m.ChatId)
            .Join(_context.Chats, chatId => chatId, c => c.Id, (chatId, c) => c)
            .AnyAsync(c => c.Participants.Any(p => p.UserId == userId), ct);

        if (reachableViaChat)
            return true;

        // 3) A user profile photo (publicly viewable).
        return await _context.Set<UserProfilePhoto>().AnyAsync(p => p.FileId == fileId, ct);
    }
}
