using MediatR;
using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationMessages;

public class GetConversationMessagesQuery : IRequest<IReadOnlyList<GetConversationMessagesResult>>
{
    public string UserId { get; set; } = string.Empty;

    public string OtherUserId { get; set; } = string.Empty;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 50;

    public MessageType? MessageType { get; set; }

    public int? ListingId { get; set; }
}
