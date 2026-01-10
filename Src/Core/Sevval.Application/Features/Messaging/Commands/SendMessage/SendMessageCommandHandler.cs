using MediatR;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Application.Features.Messaging.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    private readonly IMessageWriteRepository _messageWriteRepository;
    private readonly IMessageReadRepository _messageReadRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly IApplicationDbContext _context;

    public SendMessageCommandHandler(
        IMessageWriteRepository messageWriteRepository,
        IMessageReadRepository messageReadRepository,
        IDateTimeService dateTimeService,
        IApplicationDbContext context)
    {
        _messageWriteRepository = messageWriteRepository;
        _messageReadRepository = messageReadRepository;
        _dateTimeService = dateTimeService;
        _context = context;
    }

    public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (request.ListingId.HasValue)
        {
            var listing = await _context.IlanBilgileri
                .AsNoTracking()
                .Where(i => i.Id == request.ListingId.Value)
                .Select(i => new { i.Id, i.Email })
                .FirstOrDefaultAsync(cancellationToken);

            if (listing == null)
            {
                return new SendMessageResult
                {
                    IsSuccess = false,
                    Error = "ListingNotFound"
                };
            }

            if (string.IsNullOrWhiteSpace(listing.Email))
            {
                return new SendMessageResult
                {
                    IsSuccess = false,
                    Error = "ListingOwnerNotFound"
                };
            }

            var ownerUserId = await _context.Users
                .AsNoTracking()
                .Where(u => u.Email == listing.Email)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(ownerUserId))
            {
                return new SendMessageResult
                {
                    IsSuccess = false,
                    Error = "ListingOwnerNotFound"
                };
            }

            var isOwnerSender = string.Equals(ownerUserId, request.SenderId, StringComparison.OrdinalIgnoreCase);
            var isOwnerRecipient = string.Equals(ownerUserId, request.RecipientId, StringComparison.OrdinalIgnoreCase);

            if (!isOwnerSender && !isOwnerRecipient)
            {
                return new SendMessageResult
                {
                    IsSuccess = false,
                    Error = "ListingOwnerMismatch"
                };
            }
        }

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
            MessageStatus.Delivered,
            request.MessageType);

        message.ListingId = request.ListingId;

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
