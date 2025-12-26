using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.BusinessType.Queries.GetBusinessTypes
{
    public class GetBusinessTypesQueryRequest : IRequest<ApiResponse<List<GetBusinessTypesQueryResponse>>>
    {
        public const string Route = "/api/v1/businesss/types";

    }
}
