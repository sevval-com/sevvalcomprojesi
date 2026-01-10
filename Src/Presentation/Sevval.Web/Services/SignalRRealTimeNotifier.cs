using Microsoft.AspNetCore.SignalR;
using Sevval.Application.Features.Messaging.Notifications;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Web.Hubs;

namespace Sevval.Web.Services;

public class SignalRRealTimeNotifier : IRealTimeNotifier
{
    private readonly IHubContext<MessagingHub> _hubContext;

    public SignalRRealTimeNotifier(IHubContext<MessagingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyMessagesReadAsync(
        string userId,
        MessagesReadNotification notification,
        CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(userId)
            .SendAsync("messagesRead", notification, cancellationToken);
    }

    public Task NotifyMessagesReadUndoneAsync(
        string userId,
        MessagesReadUndoneNotification notification,
        CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(userId)
            .SendAsync("messagesReadUndone", notification, cancellationToken);
    }
}
