using Sevval.Application.Features.BuildingAge.Queries.GetBuildingAges;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService;

public interface IBuildingAgeService
{
    Task<ApiResponse<GetBuildingAgesQueryResponse>> GetBuildingAgesAsync(CancellationToken cancellationToken = default);
}
