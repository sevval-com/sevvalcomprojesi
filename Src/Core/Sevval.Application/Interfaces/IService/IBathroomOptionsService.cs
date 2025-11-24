using Sevval.Application.Features.BathroomOptions.Queries.GetBathroomOptions;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService
{
    public interface IBathroomOptionsService
    {
        public Task<ApiResponse<List<GetBathroomOptionsQueryResponse>>> GetBathroomOptionsAsync(GetBathroomOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
