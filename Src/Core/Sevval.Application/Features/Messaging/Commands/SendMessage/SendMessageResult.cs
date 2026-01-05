using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageResult
{
    public Guid MessageId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public MessageStatus Status { get; set; }
}
