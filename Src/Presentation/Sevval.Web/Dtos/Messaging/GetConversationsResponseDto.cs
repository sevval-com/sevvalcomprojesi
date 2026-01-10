namespace Sevval.Web.Dtos.Messaging;

public class GetConversationsResponseDto
{
    public List<ConversationSummaryDto> Items { get; set; } = new();
}
