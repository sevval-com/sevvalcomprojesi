namespace Sevval.Application.Features.Messaging.Notifications;

public record MessagesReadUndoneNotification(
    string ReaderId,
    IReadOnlyList<Guid> MessageIds,
    DateTime DeliveredOnUtc);
