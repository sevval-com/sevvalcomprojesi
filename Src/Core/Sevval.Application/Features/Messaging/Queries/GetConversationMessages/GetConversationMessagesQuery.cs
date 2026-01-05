using MediatR;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationMessages;

public class GetConversationMessagesQuery : IRequest<IReadOnlyList<GetConversationMessagesResult>>
{
    public string UserId { get; set; } = string.Empty;

    public string OtherUserId { get; set; } = string.Empty;
}
