using Sevval.Application.Features.Messaging.Notifications;
using Sevval.Application.Interfaces.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Services;

public sealed class NullRealTimeNotifier : IRealTimeNotifier
{
    public Task NotifyMessagesReadAsync(
        string userId,
        MessagesReadNotification notification,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task NotifyMessagesReadUndoneAsync(
        string userId,
        MessagesReadUndoneNotification notification,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
