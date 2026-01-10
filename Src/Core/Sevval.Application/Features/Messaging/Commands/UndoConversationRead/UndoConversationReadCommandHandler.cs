using MediatR;
using Sevval.Application.Features.Messaging.Notifications;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Application.Interfaces.Messaging;

namespace Sevval.Application.Features.Messaging.Commands.UndoConversationRead;

public class UndoConversationReadCommandHandler
    : IRequestHandler<UndoConversationReadCommand, UndoConversationReadResult>
{
    private readonly IMessageReadStateRepository _readStateRepository;
    private readonly IRealTimeNotifier _realTimeNotifier;
    private readonly IDateTimeService _dateTimeService;

    public UndoConversationReadCommandHandler(
        IMessageReadStateRepository readStateRepository,
        IRealTimeNotifier realTimeNotifier,
        IDateTimeService dateTimeService)
    {
        _readStateRepository = readStateRepository;
        _realTimeNotifier = realTimeNotifier;
        _dateTimeService = dateTimeService;
    }

    public async Task<UndoConversationReadResult> Handle(
        UndoConversationReadCommand request,
        CancellationToken cancellationToken)
    {
        var deliveredOnUtc = _dateTimeService.UtcNow;
        var messageIds = await _readStateRepository.UndoConversationReadAsync(
            request.ReaderId,
            request.OtherUserId,
            request.ListingId,
            request.MessageIds,
            deliveredOnUtc,
            cancellationToken);

        if (messageIds.Count > 0)
        {
            var notification = new MessagesReadUndoneNotification(
                request.ReaderId,
                messageIds,
                deliveredOnUtc);

            await _realTimeNotifier.NotifyMessagesReadUndoneAsync(
                request.OtherUserId,
                notification,
                cancellationToken);
        }

        return new UndoConversationReadResult
        {
            MessageIds = messageIds,
            DeliveredOnUtc = messageIds.Count > 0 ? deliveredOnUtc : null
        };
    }
}
