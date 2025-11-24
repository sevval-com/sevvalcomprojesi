using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.BusinessStatus.Queries.GetBusinessStatuses
{
    public class GetBusinessStatusesQueryRequest : IRequest<ApiResponse<List<GetBusinessStatusesQueryResponse>>>
    {
        public const string Route = "/api/v1/businesss/statuses";

    }
}
