using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationsForUser;

public class GetConversationsForUserResult
{
    public string OtherUserId { get; init; } = string.Empty;
    public string? OtherUserFullName { get; init; }
    public string? OtherUserEmail { get; init; }
    public string? OtherUserAvatarUrl { get; init; }

    public string LastMessagePreview { get; init; } = string.Empty;

    public DateTime LastMessageAtUtc { get; init; }

    public int UnreadCount { get; init; }

    public MessageType MessageType { get; init; }

    public int? ListingId { get; init; }
}
