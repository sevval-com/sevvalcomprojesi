namespace Sevval.Web.Dtos.Messaging;

public class GetConversationMessagesResponseDto
{
    public IReadOnlyList<MessageDto> Items { get; set; } = Array.Empty<MessageDto>();
}
