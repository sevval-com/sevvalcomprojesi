using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.FieldStatus.Queries.GetFieldStatuses
{
    public class GetFieldStatusesQueryRequest : IRequest<ApiResponse<GetFieldStatusesQueryResponse>>
    {
        public const string Route = "/api/v1/fields/statuses";

    }
}
