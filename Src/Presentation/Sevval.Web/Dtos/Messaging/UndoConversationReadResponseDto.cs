namespace Sevval.Web.Dtos.Messaging;

public class UndoConversationReadResponseDto
{
    public IReadOnlyList<Guid> MessageIds { get; set; } = Array.Empty<Guid>();

    public DateTime? DeliveredOnUtc { get; set; }
}
