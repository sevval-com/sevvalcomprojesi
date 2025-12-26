using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Neighbourhood.Queries.GetNeighbourhoods;
public class GetNeighbourhoodsQueryHandler : BaseHandler, IRequestHandler<GetNeighbourhoodsQueryRequest, ApiResponse<IList<GetNeighbourhoodsQueryResponse>>>
{
    private readonly INeighbourhoodService _neighbourhoodService;

    public GetNeighbourhoodsQueryHandler(IHttpContextAccessor httpContextAccessor, INeighbourhoodService neighbourhoodService) : base(httpContextAccessor)
    {
        _neighbourhoodService = neighbourhoodService;
    }

    public async Task<ApiResponse<IList<GetNeighbourhoodsQueryResponse>>> Handle(GetNeighbourhoodsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _neighbourhoodService.GetNeighbourhoods(request);

        return response;
    }
}
