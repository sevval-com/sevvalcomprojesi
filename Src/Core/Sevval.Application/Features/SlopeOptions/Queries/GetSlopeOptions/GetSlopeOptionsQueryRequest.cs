using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.SlopeOptions.Queries.GetSlopeOptions
{
    public class GetSlopeOptionsQueryRequest : IRequest<ApiResponse<List<GetSlopeOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/slope-options";

    }
}
