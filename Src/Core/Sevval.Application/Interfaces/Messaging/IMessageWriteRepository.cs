using Sevval.Domain.Messaging;

namespace Sevval.Application.Interfaces.Messaging;

public interface IMessageWriteRepository
{
    Task AddAsync(Message message, CancellationToken cancellationToken);
}
