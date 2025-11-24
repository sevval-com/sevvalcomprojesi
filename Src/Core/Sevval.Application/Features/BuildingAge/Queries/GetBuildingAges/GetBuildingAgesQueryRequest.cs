using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.BuildingAge.Queries.GetBuildingAges;

public class GetBuildingAgesQueryRequest : IRequest<ApiResponse<GetBuildingAgesQueryResponse>>
{
    public const string Route = "/api/v1/building-ages";
}
