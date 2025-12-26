using Sevval.Application.Features.LandStatus.Queries.GetLandStatuses;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService;

public interface ILandStatusService
{
    Task<ApiResponse<GetLandStatusesQueryResponse>> GetLandStatusesAsync(CancellationToken cancellationToken = default);
}
