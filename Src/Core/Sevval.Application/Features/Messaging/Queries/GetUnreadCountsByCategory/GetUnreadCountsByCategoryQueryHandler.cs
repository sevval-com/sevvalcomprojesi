using MediatR;
using Sevval.Application.Interfaces.Messaging;

namespace Sevval.Application.Features.Messaging.Queries.GetUnreadCountsByCategory;

public class GetUnreadCountsByCategoryQueryHandler
    : IRequestHandler<GetUnreadCountsByCategoryQuery, GetUnreadCountsByCategoryResult>
{
    private readonly IMessageReadStateRepository _readStateRepository;

    public GetUnreadCountsByCategoryQueryHandler(IMessageReadStateRepository readStateRepository)
    {
        _readStateRepository = readStateRepository;
    }

    public async Task<GetUnreadCountsByCategoryResult> Handle(
        GetUnreadCountsByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var counts = await _readStateRepository.GetUnreadCountsByCategoryAsync(
            request.UserId,
            cancellationToken);

        return new GetUnreadCountsByCategoryResult
        {
            Counts = counts
        };
    }
}
