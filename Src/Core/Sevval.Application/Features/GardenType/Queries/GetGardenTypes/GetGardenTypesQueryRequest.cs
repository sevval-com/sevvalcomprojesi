using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.GardenType.Queries.GetGardenTypes;

public class GetGardenTypesQueryRequest : IRequest<ApiResponse<List<GetGardenTypesQueryResponse>>>
{
    public const string Route = "/api/v1/garden/types";

}
