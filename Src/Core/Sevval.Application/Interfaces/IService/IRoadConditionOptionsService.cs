using Sevval.Application.DTOs.RoadConditionOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.RoadConditionOptions.Queries.GetRoadConditionOptions;

namespace Sevval.Application.Interfaces.IService
{
    public interface IRoadConditionOptionsService
    {
        Task<ApiResponse<List<GetRoadConditionOptionsQueryResponse>>> GetRoadConditionOptionsAsync(GetRoadConditionOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
