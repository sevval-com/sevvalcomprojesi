using Sevval.Application.Features.Messaging.Notifications;

namespace Sevval.Application.Interfaces.Messaging;

public interface IRealTimeNotifier
{
    Task NotifyMessagesReadAsync(
        string userId,
        MessagesReadNotification notification,
        CancellationToken cancellationToken);

    Task NotifyMessagesReadUndoneAsync(
        string userId,
        MessagesReadUndoneNotification notification,
        CancellationToken cancellationToken);
}
