using Microsoft.EntityFrameworkCore;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Persistence.Context;
using Sevval.Domain.Messaging;
using Sevval.Domain.Entities;
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
        MessageType? messageType,
        int? listingId,
        CancellationToken cancellationToken)
    {
        const int maxPageSize = 20;
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize <= 0 ? maxPageSize : Math.Min(pageSize, maxPageSize);
        var skip = (safePage - 1) * safePageSize;

        var query = Table
            .AsNoTracking()
            .Where(m =>
                (m.SenderId == participantAId && m.RecipientId == participantBId) ||
                (m.SenderId == participantBId && m.RecipientId == participantAId));

        if (listingId.HasValue)
        {
            query = query.Where(m => m.ListingId == listingId.Value);
        }
        else
        {
            query = query.Where(m => m.ListingId == null);
        }

        if (messageType.HasValue && messageType is MessageType.Comment or MessageType.Notification)
        {
            query = query.Where(m => m.MessageType == messageType.Value);
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedOnUtc)
            .ThenByDescending(m => m.Id)
            .Skip(skip)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        var participantIds = new[] { participantAId, participantBId };
        var userTypes = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => participantIds.Contains(u.Id))
            .Select(u => new { u.Id, u.UserTypes })
            .ToDictionaryAsync(u => u.Id, u => u.UserTypes, cancellationToken);

        MessageType? conversationType = null;
        if (listingId.HasValue)
        {
            var listingEmail = await _context.IlanBilgileri
                .AsNoTracking()
                .Where(i => i.Id == listingId.Value)
                .Select(i => i.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(listingEmail))
            {
                var listingOwnerId = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Email == listingEmail)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(listingOwnerId))
                {
                    conversationType = string.Equals(participantAId, listingOwnerId, StringComparison.OrdinalIgnoreCase)
                        ? MessageType.Own
                        : MessageType.Other;
                }
            }
        }

        conversationType ??= ResolveConversationType(participantAId, participantBId, userTypes);
        if (messageType is MessageType.Own or MessageType.Other)
        {
            if (conversationType != messageType)
            {
                return Array.Empty<MessagingMessage>();
            }

            foreach (var message in messages)
            {
                message.MessageType = conversationType ?? message.MessageType;
            }
        }

        return messages;
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

    public async Task<IReadOnlyList<ConversationSummary>> GetConversationsAsync(
        string userId,
        MessageType? messageType,
        CancellationToken cancellationToken)
    {
        var baseQuery = Table
            .AsNoTracking()
            .Where(m => m.SenderId == userId || m.RecipientId == userId);

        if (messageType.HasValue && messageType is MessageType.Comment or MessageType.Notification)
        {
            baseQuery = baseQuery.Where(m => m.MessageType == messageType.Value);
        }

        var latestMessages = await baseQuery
            .Select(m => new
            {
                OtherUserId = m.SenderId == userId ? m.RecipientId : m.SenderId,
                ListingId = m.ListingId,
                Message = m
            })
            .GroupBy(x => new { x.OtherUserId, x.ListingId })
            .Select(group => group
                .OrderByDescending(x => x.Message.CreatedOnUtc)
                .ThenByDescending(x => x.Message.Id)
                .Select(x => new
                {
                    x.OtherUserId,
                    x.ListingId,
                    x.Message.Body,
                    x.Message.CreatedOnUtc,
                    x.Message.MessageType
                })
                .FirstOrDefault())
            .ToListAsync(cancellationToken);

        var readStates = _context.Set<MessageReadState>().AsNoTracking();

        var filterUnreadByType = messageType.HasValue && messageType is MessageType.Comment or MessageType.Notification;
        var unreadCounts = await (from message in Table.AsNoTracking()
                                  where message.RecipientId == userId
                                  where !filterUnreadByType || message.MessageType == messageType.Value
                                  join readState in readStates
                                      on new { MessageId = message.Id, ReaderId = userId }
                                      equals new { readState.MessageId, readState.ReaderId }
                                      into readStateJoin
                                  from readState in readStateJoin.DefaultIfEmpty()
                                  where readState == null || readState.Status != MessageReadStatus.Read
                                  group message by new { message.SenderId, message.ListingId }
                                  into grouped
                                  select new
                                  {
                                      OtherUserId = grouped.Key.SenderId,
                                      grouped.Key.ListingId,
                                      Count = grouped.Count()
                                  })
            .ToListAsync(cancellationToken);

        var unreadLookup = unreadCounts.ToDictionary(x => (x.OtherUserId, x.ListingId), x => x.Count);

        var otherUserIds = latestMessages
            .Where(item => item != null)
            .Select(item => item!.OtherUserId)
            .Distinct()
            .ToList();

        var listingIds = latestMessages
            .Where(item => item != null && item!.ListingId.HasValue)
            .Select(item => item!.ListingId!.Value)
            .Distinct()
            .ToList();

        var listingEmailLookup = listingIds.Count == 0
            ? new Dictionary<int, string?>()
            : await _context.IlanBilgileri
                .AsNoTracking()
                .Where(i => listingIds.Contains(i.Id))
                .Select(i => new { i.Id, i.Email })
                .ToDictionaryAsync(i => i.Id, i => i.Email, cancellationToken);

        var listingOwnerLookup = listingEmailLookup.Count == 0
            ? new Dictionary<string, string?>()
            : await _context.Users
                .AsNoTracking()
                .Where(u => listingEmailLookup.Values.Contains(u.Email))
                .Select(u => new { u.Id, u.Email })
                .ToDictionaryAsync(u => u.Email, u => u.Id, cancellationToken);

        var userIds = otherUserIds.Concat(new[] { userId }).Distinct().ToList();
        var userLookup = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.ProfilePicturePath,
                u.UserTypes
            })
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var userTypesLookup = userLookup.ToDictionary(x => x.Key, x => x.Value.UserTypes);

        return latestMessages
            .Where(item => item != null)
            .Select(item =>
            {
                var hasUser = userLookup.TryGetValue(item!.OtherUserId, out var user);
                var currentUserType = userLookup.TryGetValue(userId, out var currentUser)
                    ? currentUser.UserTypes
                    : null;
                var listingOwnerId = item.ListingId.HasValue
                    && listingEmailLookup.TryGetValue(item.ListingId.Value, out var listingEmail)
                    && !string.IsNullOrWhiteSpace(listingEmail)
                    && listingOwnerLookup.TryGetValue(listingEmail, out var ownerId)
                    ? ownerId
                    : null;

                var conversationType = listingOwnerId != null
                    ? string.Equals(userId, listingOwnerId, StringComparison.OrdinalIgnoreCase)
                        ? MessageType.Own
                        : MessageType.Other
                    : ResolveConversationType(userId, item.OtherUserId, userTypesLookup, currentUserType);
                var fullName = hasUser ? $"{user.FirstName} {user.LastName}".Trim() : null;

                var resolvedType = conversationType ?? item.MessageType;
                if (messageType is MessageType.Own or MessageType.Other)
                {
                    if (resolvedType != messageType)
                    {
                        return null;
                    }
                }

                return new ConversationSummary
                {
                    OtherUserId = item.OtherUserId,
                    OtherUserFullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                    OtherUserEmail = hasUser ? user.Email : null,
                    OtherUserAvatarUrl = hasUser ? user.ProfilePicturePath : null,
                    LastMessagePreview = item.Body,
                    LastMessageAtUtc = item.CreatedOnUtc,
                    UnreadCount = unreadLookup.TryGetValue((item.OtherUserId, item.ListingId), out var count) ? count : 0,
                    MessageType = resolvedType,
                    ListingId = item.ListingId
                };
            })
            .Where(summary => summary != null)
            .Select(summary => summary!)
            .ToList();
    }

    private static MessageType? ResolveConversationType(
        string currentUserId,
        string otherUserId,
        IReadOnlyDictionary<string, string> userTypes,
        string? currentUserTypeOverride = null)
    {
        var currentType = currentUserTypeOverride
            ?? (userTypes.TryGetValue(currentUserId, out var currentUserType) ? currentUserType : null);
        var otherType = userTypes.TryGetValue(otherUserId, out var otherUserType) ? otherUserType : null;

        var corporateId = string.Equals(currentType, "Kurumsal", StringComparison.OrdinalIgnoreCase)
            ? currentUserId
            : string.Equals(otherType, "Kurumsal", StringComparison.OrdinalIgnoreCase)
                ? otherUserId
                : null;

        if (corporateId == null)
        {
            return null;
        }

        return string.Equals(currentUserId, corporateId, StringComparison.OrdinalIgnoreCase)
            ? MessageType.Own
            : MessageType.Other;
    }
}
