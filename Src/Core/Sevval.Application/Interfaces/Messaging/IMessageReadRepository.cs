using Sevval.Domain.Messaging;

namespace Sevval.Application.Interfaces.Messaging;

public interface IMessageReadRepository
{
    Task<IReadOnlyList<Message>> GetConversationAsync(string participantAId, string participantBId, CancellationToken cancellationToken);
}
