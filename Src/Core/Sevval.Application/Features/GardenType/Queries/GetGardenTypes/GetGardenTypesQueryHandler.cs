using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.GardenType.Queries.GetGardenTypes;

public class GetGardenTypesQueryHandler : IRequestHandler<GetGardenTypesQueryRequest, ApiResponse<List<GetGardenTypesQueryResponse>>>
{
    private readonly IGardenTypeService _gardenTypeService;

    public GetGardenTypesQueryHandler(IGardenTypeService gardenTypeService)
    {
        _gardenTypeService = gardenTypeService;
    }

    public async Task<ApiResponse<List<GetGardenTypesQueryResponse>>> Handle(GetGardenTypesQueryRequest request, CancellationToken cancellationToken)
    {

        var response = await _gardenTypeService.GetGardenTypesAsync(cancellationToken);

        return response;
    }
}
