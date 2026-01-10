using MediatR;

namespace Sevval.Application.Features.Messaging.Commands.UndoConversationRead;

public class UndoConversationReadCommand : IRequest<UndoConversationReadResult>
{
    public string ReaderId { get; set; } = string.Empty;

    public string OtherUserId { get; set; } = string.Empty;

    public IReadOnlyList<Guid>? MessageIds { get; set; }

    public int? ListingId { get; set; }
}
