using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.LandStatus.Queries.GetLandStatuses;

public class GetLandStatusesQueryRequest : IRequest<ApiResponse<GetLandStatusesQueryResponse>>
{
    public const string Route = "/api/v1/lands/statuses";

}
