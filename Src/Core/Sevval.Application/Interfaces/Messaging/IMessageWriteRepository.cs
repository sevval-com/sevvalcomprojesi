using MessagingMessage = Sevval.Domain.Messaging.Message;

namespace Sevval.Application.Interfaces.Messaging;

public interface IMessageWriteRepository
{
    Task AddAsync(MessagingMessage message, CancellationToken cancellationToken);
}
