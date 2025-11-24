using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.PropertyStatus.Queries.GetPropertyStatuses;

public class GetPropertyStatusesQueryRequest : IRequest<ApiResponse<GetPropertyStatusesQueryResponse>>
{
    public const string Route = "/api/v1/property/statuses";

}
