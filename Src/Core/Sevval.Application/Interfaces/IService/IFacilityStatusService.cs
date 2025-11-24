using Sevval.Application.Features.Common;
using Sevval.Application.Features.FacilityStatus.Queries.GetFacilityStatuses;

namespace Sevval.Application.Interfaces.IService
{
    public interface IFacilityStatusService
    {
        Task<ApiResponse<List<GetFacilityStatusesQueryResponse>>> GetFacilityStatusesAsync(GetFacilityStatusesQueryRequest request, CancellationToken cancellationToken = default);
    }
}
