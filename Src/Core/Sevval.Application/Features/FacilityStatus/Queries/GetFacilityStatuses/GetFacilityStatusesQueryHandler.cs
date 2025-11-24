using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.FacilityStatus.Queries.GetFacilityStatuses;

public class GetFacilityStatusesQueryHandler : IRequestHandler<GetFacilityStatusesQueryRequest, ApiResponse<List<GetFacilityStatusesQueryResponse>>>
{
    private readonly IFacilityStatusService _facilityStatusService;

    public GetFacilityStatusesQueryHandler(IFacilityStatusService facilityStatusService)
    {
        _facilityStatusService = facilityStatusService;
    }

    public async Task<ApiResponse<List<GetFacilityStatusesQueryResponse>>> Handle(GetFacilityStatusesQueryRequest request, CancellationToken cancellationToken)
    {
        return await _facilityStatusService.GetFacilityStatusesAsync(request, cancellationToken);
    }
}
