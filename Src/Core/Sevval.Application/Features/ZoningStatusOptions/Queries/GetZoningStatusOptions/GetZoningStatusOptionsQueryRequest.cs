using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.ZoningStatusOptions.Queries.GetZoningStatusOptions
{
    public class GetZoningStatusOptionsQueryRequest : IRequest<ApiResponse<List<GetZoningStatusOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/zoning-status-options";

    }
}
