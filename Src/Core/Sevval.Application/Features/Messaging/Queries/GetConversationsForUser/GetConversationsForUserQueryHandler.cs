using MediatR;
using Sevval.Application.Interfaces.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationsForUser;

public class GetConversationsForUserQueryHandler
    : IRequestHandler<GetConversationsForUserQuery, IReadOnlyList<GetConversationsForUserResult>>
{
    private readonly IMessageReadRepository _messageReadRepository;

    public GetConversationsForUserQueryHandler(IMessageReadRepository messageReadRepository)
    {
        _messageReadRepository = messageReadRepository;
    }

    public async Task<IReadOnlyList<GetConversationsForUserResult>> Handle(
        GetConversationsForUserQuery request,
        CancellationToken cancellationToken)
    {
        var summaries = await _messageReadRepository.GetConversationsAsync(
            request.UserId,
            request.MessageType,
            cancellationToken);

        return summaries
            .OrderByDescending(s => s.LastMessageAtUtc)
            .ThenBy(s => s.OtherUserId)
            .Select(summary => new GetConversationsForUserResult
            {
                OtherUserId = summary.OtherUserId,
                OtherUserFullName = summary.OtherUserFullName,
                OtherUserEmail = summary.OtherUserEmail,
                OtherUserAvatarUrl = summary.OtherUserAvatarUrl,
                LastMessagePreview = summary.LastMessagePreview,
                LastMessageAtUtc = summary.LastMessageAtUtc,
                UnreadCount = summary.UnreadCount,
                MessageType = summary.MessageType,
                ListingId = summary.ListingId
            })
            .ToList();
    }
}
