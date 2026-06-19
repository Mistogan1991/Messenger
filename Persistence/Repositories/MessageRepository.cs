using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Features.Messages.Dtos;
using Messenger.Domain.Aggregates.Messages;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories
{
    public sealed class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Message message, CancellationToken ct)
        {
            await _context.Messages.AddAsync(message, ct);
        }

        public async Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<List<MessageDto>> GetChatMessagesAsync(
            Guid chatId, DateTime? before, int limit, CancellationToken ct = default)
        {
            // Soft-delete query filter excludes deleted messages automatically.
            var query = _context.Messages.Where(m => m.ChatId == chatId);

            if (before is not null)
                query = query.Where(m => m.CreatedAtUtc < before.Value);

            return await query
                .OrderByDescending(m => m.CreatedAtUtc)
                .Take(limit)
                .Select(m => new MessageDto(
                    m.Id,
                    m.ChatId,
                    m.SenderId,
                    m.Type,
                    m.Content,
                    m.ReplyToMessageId,
                    m.IsEdited,
                    m.EditedAtUtc,
                    m.CreatedAtUtc))
                .ToListAsync(ct);
        }
    }
}
