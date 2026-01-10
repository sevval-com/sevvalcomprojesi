namespace Sevval.Web.Dtos.Messaging;

public class MarkConversationAsReadResponseDto
{
    public IReadOnlyList<Guid> MessageIds { get; set; } = Array.Empty<Guid>();

    public DateTime ReadOnUtc { get; set; }
}
