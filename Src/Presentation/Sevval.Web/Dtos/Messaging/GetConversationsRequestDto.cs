using Sevval.Domain.Messaging;

namespace Sevval.Web.Dtos.Messaging;

public class GetConversationsRequestDto
{
    public string UserId { get; set; } = string.Empty;

    public MessageType? MessageType { get; set; }
}
