using Sevval.Application.Features.GardenStatus.Queries.GetGardenStatuses;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService;

public interface IGardenStatusService
{
    Task<ApiResponse<GetGardenStatusesQueryResponse>> GetGardenStatusesAsync(CancellationToken cancellationToken = default);
}
