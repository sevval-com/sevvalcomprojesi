using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.FacilityType.Queries.GetFacilityTypes;

public class GetFacilityTypesQueryHandler : IRequestHandler<GetFacilityTypesQueryRequest, ApiResponse<List<GetFacilityTypesQueryResponse>>>
{
    private readonly IFacilityTypeService _facilityTypeService;

    public GetFacilityTypesQueryHandler(IFacilityTypeService facilityTypeService)
    {
        _facilityTypeService = facilityTypeService;
    }

    public async Task<ApiResponse<List<GetFacilityTypesQueryResponse>>> Handle(GetFacilityTypesQueryRequest request, CancellationToken cancellationToken)
    {
        return await _facilityTypeService.GetFacilityTypesAsync(request, cancellationToken);
    }
}
