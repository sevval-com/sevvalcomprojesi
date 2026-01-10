namespace Sevval.Application.Features.Messaging.Notifications;

public record MessagesReadNotification(
    string ReaderId,
    IReadOnlyList<Guid> MessageIds,
    DateTime ReadOnUtc);
