using MediatR;
using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationsForUser;

public class GetConversationsForUserQuery : IRequest<IReadOnlyList<GetConversationsForUserResult>>
{
    public string UserId { get; set; } = string.Empty;

    public MessageType? MessageType { get; set; }
}
