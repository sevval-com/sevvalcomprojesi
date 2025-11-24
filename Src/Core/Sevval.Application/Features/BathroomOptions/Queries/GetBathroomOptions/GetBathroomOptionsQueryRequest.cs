using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.BathroomOptions.Queries.GetBathroomOptions
{
    public class GetBathroomOptionsQueryRequest : IRequest<ApiResponse<List<GetBathroomOptionsQueryResponse>>>
    {
        public const string Route = "/api/v1/bathroom-options";

    }
}
