using Sevval.Application.Features.Common;
using Sevval.Application.Features.FloorOptions.Queries.GetFloorOptions;

namespace Sevval.Application.Interfaces.IService
{
    public interface IFloorOptionsService
    {
        Task<ApiResponse<List<GetFloorOptionsQueryResponse>>> GetFloorOptionsAsync(GetFloorOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
