using MediatR;
using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommand : IRequest<SendMessageResult>
{
    public string SenderId { get; set; } = string.Empty;

    public string RecipientId { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}
