using Microsoft.EntityFrameworkCore;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;
using Sevval.Persistence.Context;
using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Persistence.Repositories.Messaging;

public class MessageReadStateRepository : IMessageReadStateRepository
{
    private readonly ApplicationDbContext _context;
    private DbSet<MessageReadState> ReadStates => _context.Set<MessageReadState>();
    private DbSet<MessagingMessage> Messages => _context.Set<MessagingMessage>();

    public MessageReadStateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyDictionary<MessageType, int>> GetUnreadCountsByCategoryAsync(
        string readerId,
        CancellationToken cancellationToken)
    {
        var readerType = await _context.Set<Sevval.Domain.Entities.ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == readerId)
            .Select(u => u.UserTypes)
            .FirstOrDefaultAsync(cancellationToken);

        var unreadTypes = await (from message in Messages.AsNoTracking()
                                 where message.RecipientId == readerId
                                 join state in ReadStates.AsNoTracking()
                                     on new { MessageId = message.Id, ReaderId = readerId }
                                     equals new { MessageId = state.MessageId, ReaderId = state.ReaderId }
                                     into stateGroup
                                 from state in stateGroup.DefaultIfEmpty()
                                 where state == null || state.Status != MessageReadStatus.Read
                                 select message.MessageType)
            .ToListAsync(cancellationToken);

        var counts = new Dictionary<MessageType, int>();

        foreach (var messageType in unreadTypes)
        {
            var effectiveType = messageType;
            if (messageType is not (MessageType.Comment or MessageType.Notification))
            {
                effectiveType = string.Equals(readerType, "Kurumsal", StringComparison.OrdinalIgnoreCase)
                    ? MessageType.Own
                    : MessageType.Other;
            }

            counts[effectiveType] = counts.TryGetValue(effectiveType, out var current)
                ? current + 1
                : 1;
        }

        foreach (var messageType in Enum.GetValues<MessageType>())
        {
            if (!counts.ContainsKey(messageType))
            {
                counts[messageType] = 0;
            }
        }

        return counts;
    }

    public async Task<IReadOnlyList<Guid>> MarkConversationAsReadAsync(
        string readerId,
        string otherUserId,
        int? listingId,
        DateTime readOnUtc,
        CancellationToken cancellationToken)
    {
        var baseQuery = Messages.AsNoTracking()
            .Where(m => m.SenderId == otherUserId && m.RecipientId == readerId);

        if (listingId.HasValue)
        {
            baseQuery = baseQuery.Where(m => m.ListingId == listingId.Value);
        }
        else
        {
            baseQuery = baseQuery.Where(m => m.ListingId == null);
        }

        var messageIds = await baseQuery
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        if (messageIds.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        var existingStates = await ReadStates
            .Where(s => s.ReaderId == readerId && messageIds.Contains(s.MessageId))
            .ToListAsync(cancellationToken);

        var existingByMessageId = existingStates.ToDictionary(s => s.MessageId);
        var updatedMessageIds = new List<Guid>();

        foreach (var messageId in messageIds)
        {
            if (existingByMessageId.TryGetValue(messageId, out var state))
            {
                if (state.Status == MessageReadStatus.Read)
                {
                    continue;
                }

                state.Status = MessageReadStatus.Read;
                state.ReadOnUtc = readOnUtc;
                state.DeliveredOnUtc ??= readOnUtc;
                updatedMessageIds.Add(messageId);
                continue;
            }

            var newState = new MessageReadState
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                ReaderId = readerId,
                Status = MessageReadStatus.Read,
                DeliveredOnUtc = readOnUtc,
                ReadOnUtc = readOnUtc
            };

            ReadStates.Add(newState);
            updatedMessageIds.Add(messageId);
        }

        if (updatedMessageIds.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return updatedMessageIds;
    }

    public async Task<IReadOnlyList<Guid>> UndoConversationReadAsync(
        string readerId,
        string otherUserId,
        int? listingId,
        IReadOnlyCollection<Guid>? messageIds,
        DateTime deliveredOnUtc,
        CancellationToken cancellationToken)
    {
        if (messageIds is { Count: 0 })
        {
            return Array.Empty<Guid>();
        }

        var baseQuery = Messages.AsNoTracking()
            .Where(m => m.SenderId == otherUserId && m.RecipientId == readerId);

        if (listingId.HasValue)
        {
            baseQuery = baseQuery.Where(m => m.ListingId == listingId.Value);
        }
        else
        {
            baseQuery = baseQuery.Where(m => m.ListingId == null);
        }

        var candidateMessageIds = await baseQuery
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        if (candidateMessageIds.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        if (messageIds != null)
        {
            candidateMessageIds = candidateMessageIds
                .Where(id => messageIds.Contains(id))
                .ToList();
        }

        if (candidateMessageIds.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        var statesToUpdate = await ReadStates
            .Where(s =>
                s.ReaderId == readerId &&
                candidateMessageIds.Contains(s.MessageId) &&
                s.Status == MessageReadStatus.Read)
            .ToListAsync(cancellationToken);

        if (statesToUpdate.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        foreach (var state in statesToUpdate)
        {
            state.Status = MessageReadStatus.Delivered;
            state.DeliveredOnUtc = deliveredOnUtc;
            state.ReadOnUtc = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return statesToUpdate.Select(s => s.MessageId).ToList();
    }

    public async Task<IReadOnlyCollection<Guid>> GetReadMessageIdsAsync(
        string readerId,
        IReadOnlyCollection<Guid> messageIds,
        CancellationToken cancellationToken)
    {
        if (messageIds.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        return await ReadStates
            .AsNoTracking()
            .Where(s => s.ReaderId == readerId
                        && messageIds.Contains(s.MessageId)
                        && s.Status == MessageReadStatus.Read)
            .Select(s => s.MessageId)
            .ToListAsync(cancellationToken);
    }
}
