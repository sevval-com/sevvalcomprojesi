using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.FacilityType.Queries.GetFacilityTypes;

public class GetFacilityTypesQueryRequest : IRequest<ApiResponse<List<GetFacilityTypesQueryResponse>>>
{
    public const string Route = "/api/v1/facility/types";

}
