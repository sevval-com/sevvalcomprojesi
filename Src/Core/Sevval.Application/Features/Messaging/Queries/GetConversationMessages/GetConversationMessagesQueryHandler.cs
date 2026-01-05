using MediatR;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationMessages;

public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<GetConversationMessagesResult>>
{
    private readonly IMessageReadRepository _messageReadRepository;

    public GetConversationMessagesQueryHandler(IMessageReadRepository messageReadRepository)
    {
        _messageReadRepository = messageReadRepository;
    }

    public async Task<IReadOnlyList<GetConversationMessagesResult>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageReadRepository.GetConversationAsync(request.UserId, request.OtherUserId, cancellationToken);

        var ordered = messages
            .OrderByDescending(m => m.CreatedOnUtc)
            .ThenByDescending(m => m.Id)
            .Select(Map)
            .ToList();

        return ordered;
    }

    private static GetConversationMessagesResult Map(Message message) => new()
    {
        Id = message.Id,
        SenderId = message.SenderId,
        RecipientId = message.RecipientId,
        Body = message.Body,
        CreatedOnUtc = message.CreatedOnUtc,
        Status = message.Status
    };
}
