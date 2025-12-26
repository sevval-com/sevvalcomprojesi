using Sevval.Application.Features.Common;
using MediatR;

namespace Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount
{
    public class GetTotalVisitorCountQueryRequest : IRequest<ApiResponse<GetTotalVisitorCountQueryResponse>>
    {
        public const string Route = "/api/v1/visitors/total-count";
    }
}
