using Sevval.Application.DTOs.DistanceToSettlementOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.DistanceToSettlementOptions.Queries.GetDistanceToSettlementOptions;

namespace Sevval.Application.Interfaces.IService
{
    public interface IDistanceToSettlementOptionsService
    {
        public Task<ApiResponse<List<GetDistanceToSettlementOptionsQueryResponse>>> GetDistanceToSettlementOptionsAsync(GetDistanceToSettlementOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
