namespace Sevval.Web.Dtos.Messaging;

public class UndoConversationReadRequestDto
{
    public string ReaderId { get; set; } = string.Empty;

    public string OtherUserId { get; set; } = string.Empty;

    public IReadOnlyList<Guid>? MessageIds { get; set; }

    public int? ListingId { get; set; }
}
