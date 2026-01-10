using Sevval.Domain.Messaging;

namespace Sevval.Web.Dtos.Messaging;

public class GetConversationMessagesRequestDto
{
    public string UserId { get; set; } = string.Empty;

    public string OtherUserId { get; set; } = string.Empty;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 50;

    public MessageType? MessageType { get; set; }

    public int? ListingId { get; set; }
}
