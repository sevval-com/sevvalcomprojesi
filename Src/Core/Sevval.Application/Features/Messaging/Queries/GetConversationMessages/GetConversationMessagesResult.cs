using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetConversationMessages;

public class GetConversationMessagesResult
{
    public Guid Id { get; set; }

    public string SenderId { get; set; } = string.Empty;

    public string RecipientId { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public DateTime CreatedOnUtc { get; set; }

    public MessageStatus Status { get; set; }

    public MessageType MessageType { get; set; } = MessageType.Other;

    public bool IsRead { get; set; }
}
