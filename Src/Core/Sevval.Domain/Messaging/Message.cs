using Sevval.Domain.Entities.Common;

namespace Sevval.Domain.Messaging;

public class Message : BaseAuditableEntity<Guid>
{
    public Message() { }

    public Message(Guid id, string senderId, string recipientId, string body, DateTime createdOnUtc, MessageStatus status)
        : base(id)
    {
        SenderId = senderId;
        RecipientId = recipientId;
        Body = body;
        CreatedOnUtc = createdOnUtc;
        Status = status;
    }

    public string SenderId { get; set; } = string.Empty;

    public string RecipientId { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    // Server-side oluşturma zamanı, sıralama ve limitler için kullan
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

    public MessageStatus Status { get; set; } = MessageStatus.Pending;
}
