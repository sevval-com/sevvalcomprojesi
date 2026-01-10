using System.Linq;
using MediatR;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;
using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationMessages;

public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<GetConversationMessagesResult>>
{
    private readonly IMessageReadRepository _messageReadRepository;
    private readonly IMessageReadStateRepository _messageReadStateRepository;

    public GetConversationMessagesQueryHandler(
        IMessageReadRepository messageReadRepository,
        IMessageReadStateRepository messageReadStateRepository)
    {
        _messageReadRepository = messageReadRepository;
        _messageReadStateRepository = messageReadStateRepository;
    }

    public async Task<IReadOnlyList<GetConversationMessagesResult>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageReadRepository.GetConversationAsync(
            request.UserId,
            request.OtherUserId,
            request.Page,
            request.PageSize,
            request.MessageType,
            request.ListingId,
            cancellationToken);

        if (messages.Count == 0)
        {
            return Array.Empty<GetConversationMessagesResult>();
        }

        var outgoingMessageIds = messages
            .Where(m => m.SenderId == request.UserId && m.RecipientId == request.OtherUserId)
            .Select(m => m.Id)
            .ToList();

        var readIds = outgoingMessageIds.Count == 0
            ? Array.Empty<Guid>()
            : await _messageReadStateRepository.GetReadMessageIdsAsync(
                request.OtherUserId,
                outgoingMessageIds,
                cancellationToken);

        var readLookup = readIds.ToHashSet();

        return messages.Select(message => Map(message, readLookup)).ToList();
    }

    private static GetConversationMessagesResult Map(
        MessagingMessage message,
        IReadOnlySet<Guid> readMessageIds) => new()
    {
        Id = message.Id,
        SenderId = message.SenderId,
        RecipientId = message.RecipientId,
        Body = message.Body,
        CreatedOnUtc = message.CreatedOnUtc,
        Status = message.Status,
        MessageType = message.MessageType,
        IsRead = readMessageIds.Contains(message.Id)
    };
}
