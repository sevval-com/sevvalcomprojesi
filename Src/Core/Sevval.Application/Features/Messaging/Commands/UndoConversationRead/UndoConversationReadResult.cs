namespace Sevval.Application.Features.Messaging.Commands.UndoConversationRead;

public class UndoConversationReadResult
{
    public IReadOnlyList<Guid> MessageIds { get; init; } = Array.Empty<Guid>();

    public DateTime? DeliveredOnUtc { get; init; }
}
