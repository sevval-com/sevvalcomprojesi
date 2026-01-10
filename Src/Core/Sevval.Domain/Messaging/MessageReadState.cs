using Sevval.Domain.Entities.Common;

namespace Sevval.Domain.Messaging;

public class MessageReadState : BaseAuditableEntity<Guid>
{
    public Guid MessageId { get; set; }

    public string ReaderId { get; set; } = string.Empty;

    public MessageReadStatus Status { get; set; } = MessageReadStatus.Delivered;

    public DateTime? DeliveredOnUtc { get; set; }

    public DateTime? ReadOnUtc { get; set; }
}
