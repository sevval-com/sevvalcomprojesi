using MediatR;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;
using MessagingMessage = Sevval.Domain.Messaging.Message;

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
        var messages = await _messageReadRepository.GetConversationAsync(
            request.UserId,
            request.OtherUserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return messages.Select(Map).ToList();
    }

    private static GetConversationMessagesResult Map(MessagingMessage message) => new()
    {
        Id = message.Id,
        SenderId = message.SenderId,
        RecipientId = message.RecipientId,
        Body = message.Body,
        CreatedOnUtc = message.CreatedOnUtc,
        Status = message.Status
    };
}
