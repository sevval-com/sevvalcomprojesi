using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.FacilityStatus.Queries.GetFacilityStatuses
{
    public class GetFacilityStatusesQueryRequest : IRequest<ApiResponse<List<GetFacilityStatusesQueryResponse>>>
    {
        public const string Route = "/api/v1/facility/statuses";
    }
}
