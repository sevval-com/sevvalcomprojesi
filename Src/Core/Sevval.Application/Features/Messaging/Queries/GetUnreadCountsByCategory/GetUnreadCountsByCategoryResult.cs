using Sevval.Domain.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetUnreadCountsByCategory;

public class GetUnreadCountsByCategoryResult
{
    public IReadOnlyDictionary<MessageType, int> Counts { get; init; } =
        new Dictionary<MessageType, int>();
}
