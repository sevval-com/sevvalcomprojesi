using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.FloorOptions.Queries.GetFloorOptions
{
    public class GetFloorOptionsQueryRequest : IRequest<ApiResponse<List<GetFloorOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/floor-options";

    }
}
