using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Features.Chats.Dtos;
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

    public async Task<Chat?> GetSavedMessagesChatAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Chats
            .Where(c => c.Type == ChatType.SavedMessages)
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<MyChatDto>> GetUserChatsAsync(Guid userId, CancellationToken ct)
    {
        // Soft-delete query filters exclude deleted chats, participants and messages automatically.
        return await _context.Chats
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Select(c => new
            {
                Chat = c,
                Me = c.Participants.First(p => p.UserId == userId),
                LastMessage = _context.Messages
                    .Where(m => m.ChatId == c.Id)
                    .OrderByDescending(m => m.CreatedAtUtc)
                    .FirstOrDefault()
            })
            .Select(x => new MyChatDto(
                x.Chat.Id,
                x.Chat.Type,
                x.Chat.Title,
                x.Chat.Username,
                x.Chat.Participants.Count,
                x.Me.IsMuted,
                x.Me.IsArchived,
                x.Me.IsPinned,
                x.Me.LastReadMessageId,
                x.LastMessage == null ? null : x.LastMessage.Content,
                x.LastMessage == null ? (DateTime?)null : x.LastMessage.CreatedAtUtc,
                x.LastMessage == null ? (Guid?)null : x.LastMessage.SenderId))
            .OrderByDescending(x => x.LastMessageAtUtc)
            .ToListAsync(ct);
    }

    public Task<bool> IsParticipantAsync(Guid chatId, Guid userId, CancellationToken ct)
    {
        return _context.Chats
            .AnyAsync(c => c.Id == chatId && c.Participants.Any(p => p.UserId == userId), ct);
    }

    public async Task<List<ChatMemberDto>> GetMembersAsync(Guid chatId, CancellationToken ct)
    {
        return await _context.Chats
            .Where(c => c.Id == chatId)
            .SelectMany(c => c.Participants)
            .Join(_context.Users,
                p => p.UserId,
                u => u.Id,
                // Username is a value-converted VO; projecting u.Username.Value isn't reliably
                // SQL-translatable, so it's left null here (resolve separately if needed).
                (p, u) => new ChatMemberDto(
                    p.UserId,
                    null,
                    u.FirstName,
                    u.LastName,
                    p.Role))
            .ToListAsync(ct);
    }
}
