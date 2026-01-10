using MediatR;

namespace Sevval.Application.Features.Messaging.Commands.MarkConversationAsRead;

public class MarkConversationAsReadCommand : IRequest<MarkConversationAsReadResult>
{
    public string ReaderId { get; set; } = string.Empty;

    public string OtherUserId { get; set; } = string.Empty;

    public int? ListingId { get; set; }
}
