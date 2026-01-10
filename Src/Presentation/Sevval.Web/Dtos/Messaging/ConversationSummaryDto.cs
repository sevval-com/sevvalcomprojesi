namespace Sevval.Web.Dtos.Messaging;

public class ConversationSummaryDto
{
    public string OtherUserId { get; set; } = string.Empty;
    public string? OtherUserFullName { get; set; }
    public string? OtherUserEmail { get; set; }
    public string? OtherUserAvatarUrl { get; set; }

    public string LastMessagePreview { get; set; } = string.Empty;

    public DateTime LastMessageAtUtc { get; set; }

    public int UnreadCount { get; set; }

    public string MessageType { get; set; } = string.Empty;

    public int? ListingId { get; set; }
}
