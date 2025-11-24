using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.DistanceToSettlementOptions.Queries.GetDistanceToSettlementOptions
{
    public class GetDistanceToSettlementOptionsQueryRequest : IRequest<ApiResponse<List<GetDistanceToSettlementOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/distance-to-settlement-options";

    }
}
