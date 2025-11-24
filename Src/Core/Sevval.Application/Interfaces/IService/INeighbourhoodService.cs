using Sevval.Application.Features.Common;
using Sevval.Application.Features.Neighbourhood.Queries.GetNeighbourhoods;

namespace Sevval.Application.Interfaces.IService;

public interface INeighbourhoodService
{
    Task<ApiResponse<IList<GetNeighbourhoodsQueryResponse>>> GetNeighbourhoods(GetNeighbourhoodsQueryRequest request);
}
