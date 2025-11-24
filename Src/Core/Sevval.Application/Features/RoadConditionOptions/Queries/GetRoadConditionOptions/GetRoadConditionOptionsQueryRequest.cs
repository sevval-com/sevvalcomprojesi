using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.RoadConditionOptions.Queries.GetRoadConditionOptions
{
    public class GetRoadConditionOptionsQueryRequest : IRequest<ApiResponse<List<GetRoadConditionOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/road-condition-options";

    }
}
