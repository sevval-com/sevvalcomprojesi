using Sevval.Domain.Messaging;

namespace Sevval.Application.Interfaces.Messaging;

public interface IMessageReadStateRepository
{
    Task<IReadOnlyDictionary<MessageType, int>> GetUnreadCountsByCategoryAsync(
        string readerId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> MarkConversationAsReadAsync(
        string readerId,
        string otherUserId,
        int? listingId,
        DateTime readOnUtc,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> UndoConversationReadAsync(
        string readerId,
        string otherUserId,
        int? listingId,
        IReadOnlyCollection<Guid>? messageIds,
        DateTime deliveredOnUtc,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Guid>> GetReadMessageIdsAsync(
        string readerId,
        IReadOnlyCollection<Guid> messageIds,
        CancellationToken cancellationToken);
}
