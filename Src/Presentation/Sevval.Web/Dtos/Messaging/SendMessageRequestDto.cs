using Sevval.Domain.Messaging;

namespace Sevval.Web.Dtos.Messaging;

public class SendMessageRequestDto
{
    public string SenderId { get; set; } = string.Empty;

    public string RecipientId { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public MessageType? MessageType { get; set; }

    public int? ListingId { get; set; }
}
