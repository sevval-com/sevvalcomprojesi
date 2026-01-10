using MediatR;
using Sevval.Application.Features.Messaging.Notifications;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Application.Interfaces.Messaging;

namespace Sevval.Application.Features.Messaging.Commands.MarkConversationAsRead;

public class MarkConversationAsReadCommandHandler
    : IRequestHandler<MarkConversationAsReadCommand, MarkConversationAsReadResult>
{
    private readonly IMessageReadStateRepository _readStateRepository;
    private readonly IRealTimeNotifier _realTimeNotifier;
    private readonly IDateTimeService _dateTimeService;

    public MarkConversationAsReadCommandHandler(
        IMessageReadStateRepository readStateRepository,
        IRealTimeNotifier realTimeNotifier,
        IDateTimeService dateTimeService)
    {
        _readStateRepository = readStateRepository;
        _realTimeNotifier = realTimeNotifier;
        _dateTimeService = dateTimeService;
    }

    public async Task<MarkConversationAsReadResult> Handle(
        MarkConversationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var readOnUtc = _dateTimeService.UtcNow;
        var messageIds = await _readStateRepository.MarkConversationAsReadAsync(
            request.ReaderId,
            request.OtherUserId,
            request.ListingId,
            readOnUtc,
            cancellationToken);

        if (messageIds.Count > 0)
        {
            var notification = new MessagesReadNotification(
                request.ReaderId,
                messageIds,
                readOnUtc);

            await _realTimeNotifier.NotifyMessagesReadAsync(
                request.OtherUserId,
                notification,
                cancellationToken);
        }

        return new MarkConversationAsReadResult
        {
            MessageIds = messageIds,
            ReadOnUtc = readOnUtc
        };
    }
}
