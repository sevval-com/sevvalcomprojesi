namespace Sevval.Web.Dtos.Messaging;

public class MarkConversationAsReadRequestDto
{
    public string ReaderId { get; set; } = string.Empty;

    public string OtherUserId { get; set; } = string.Empty;

    public int? ListingId { get; set; }
}
