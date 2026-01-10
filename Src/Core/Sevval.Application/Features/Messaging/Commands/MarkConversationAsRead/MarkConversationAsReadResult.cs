namespace Sevval.Application.Features.Messaging.Commands.MarkConversationAsRead;

public class MarkConversationAsReadResult
{
    public IReadOnlyList<Guid> MessageIds { get; init; } = Array.Empty<Guid>();

    public DateTime ReadOnUtc { get; init; }
}
