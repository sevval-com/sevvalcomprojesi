using Sevval.Application.Features.Common;
using MediatR;

namespace Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount
{
    public class GetActiveVisitorCountQueryRequest : IRequest<ApiResponse<GetActiveVisitorCountQueryResponse>>
    {
        public const string Route = "/api/v1/visitors/active-count";
    }
}
