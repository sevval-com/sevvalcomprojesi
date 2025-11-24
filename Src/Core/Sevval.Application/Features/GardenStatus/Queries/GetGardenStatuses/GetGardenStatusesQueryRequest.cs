using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.GardenStatus.Queries.GetGardenStatuses;

public class GetGardenStatusesQueryRequest : IRequest<ApiResponse<GetGardenStatusesQueryResponse>>
{
    public const string Route = "/api/v1/garden/statuses";

}
