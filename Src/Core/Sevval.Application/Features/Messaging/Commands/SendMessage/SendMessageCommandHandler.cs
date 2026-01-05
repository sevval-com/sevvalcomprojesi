using MediatR;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;
using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    private readonly IMessageWriteRepository _messageWriteRepository;
    private readonly IMessageReadRepository _messageReadRepository;
    private readonly IDateTimeService _dateTimeService;

    public SendMessageCommandHandler(
        IMessageWriteRepository messageWriteRepository,
        IMessageReadRepository messageReadRepository,
        IDateTimeService dateTimeService)
    {
        _messageWriteRepository = messageWriteRepository;
        _messageReadRepository = messageReadRepository;
        _dateTimeService = dateTimeService;
    }

    public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var createdOnUtc = _dateTimeService.UtcNow;
        var dayStartUtc = createdOnUtc.Date;
        var dayEndUtc = dayStartUtc.AddDays(1);

        var sentCountToday = await _messageReadRepository.CountSentAsync(
            request.SenderId,
            dayStartUtc,
            dayEndUtc,
            cancellationToken);

        if (sentCountToday >= 1000)
        {
            return new SendMessageResult
            {
                IsSuccess = false,
                Error = "Daily message limit exceeded."
            };
        }

        var message = new MessagingMessage(
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
            Status = message.Status,
            IsSuccess = true
        };
    }
}
