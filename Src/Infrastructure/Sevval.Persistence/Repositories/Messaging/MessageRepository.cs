using Microsoft.EntityFrameworkCore;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Persistence.Context;
using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Persistence.Repositories.Messaging;

public class MessageRepository : IMessageReadRepository, IMessageWriteRepository
{
    private readonly ApplicationDbContext _context;
    private DbSet<MessagingMessage> Table => _context.Set<MessagingMessage>();

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MessagingMessage message, CancellationToken cancellationToken)
    {
        await Table.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MessagingMessage>> GetConversationAsync(
        string participantAId,
        string participantBId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize <= 0 ? 50 : pageSize;
        var skip = (safePage - 1) * safePageSize;

        return await Table
            .AsNoTracking()
            .Where(m =>
                (m.SenderId == participantAId && m.RecipientId == participantBId) ||
                (m.SenderId == participantBId && m.RecipientId == participantAId))
            .OrderByDescending(m => m.CreatedOnUtc)
            .ThenByDescending(m => m.Id)
            .Skip(skip)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSentAsync(
        string senderId,
        DateTime fromUtcInclusive,
        DateTime toUtcExclusive,
        CancellationToken cancellationToken)
    {
        return await Table
            .AsNoTracking()
            .Where(m => m.SenderId == senderId &&
                        m.CreatedOnUtc >= fromUtcInclusive &&
                        m.CreatedOnUtc < toUtcExclusive)
            .CountAsync(cancellationToken);
    }
}
