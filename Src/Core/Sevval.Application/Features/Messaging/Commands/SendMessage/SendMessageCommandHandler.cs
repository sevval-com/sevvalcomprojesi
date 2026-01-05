using MediatR;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    private readonly IMessageWriteRepository _messageWriteRepository;
    private readonly IDateTimeService _dateTimeService;

    public SendMessageCommandHandler(
        IMessageWriteRepository messageWriteRepository,
        IDateTimeService dateTimeService)
    {
        _messageWriteRepository = messageWriteRepository;
        _dateTimeService = dateTimeService;
    }

    public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var createdOnUtc = _dateTimeService.UtcNow;

        var message = new Message(
            Guid.NewGuid(),
            request.SenderId,
            request.RecipientId,
            request.Body,
            createdOnUtc,
            MessageStatus.Delivered);

        await _messageWriteRepository.AddAsync(message, cancellationToken);

        return new SendMessageResult
        {
            MessageId = message.Id,
            CreatedOnUtc = message.CreatedOnUtc,
            Status = message.Status
        };
    }
}
