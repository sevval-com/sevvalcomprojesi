using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Application.Interfaces.Messaging;

public interface IMessageReadRepository
{
    /// <summary>
    /// Returns conversation messages between two participants ordered newest-first and paged.
    /// </summary>
    Task<IReadOnlyList<MessagingMessage>> GetConversationAsync(
        string participantAId,
        string participantBId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Counts messages sent by the given sender within the UTC range [fromUtcInclusive, toUtcExclusive).
    /// </summary>
    Task<int> CountSentAsync(
        string senderId,
        DateTime fromUtcInclusive,
        DateTime toUtcExclusive,
        CancellationToken cancellationToken);
}
