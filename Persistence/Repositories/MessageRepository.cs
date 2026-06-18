using Messenger.Application.Common.Interfaces.Repositories;
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
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
