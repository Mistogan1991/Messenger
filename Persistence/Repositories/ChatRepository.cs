using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public sealed class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Chat chat, CancellationToken ct)
    {
        await _context.Chats.AddAsync(chat, ct);
    }

    public async Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Chat?> GetPrivateChatAsync(Guid userA, Guid userB, CancellationToken ct)
    {
        return await _context.Chats
            .Where(c => c.Type == ChatType.Private)
            .Where(c => c.Participants.Any(p => p.UserId == userA)
                     && c.Participants.Any(p => p.UserId == userB))
            .FirstOrDefaultAsync(ct);
    }
}
