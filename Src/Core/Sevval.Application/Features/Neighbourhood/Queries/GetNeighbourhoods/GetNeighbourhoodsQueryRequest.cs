using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Neighbourhood.Queries.GetNeighbourhoods
{
    public class GetNeighbourhoodsQueryRequest : IRequest<ApiResponse<IList<GetNeighbourhoodsQueryResponse>>>
    {
        public const string Route = "/api/v1/neighbourhoods";

        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
    }
}
