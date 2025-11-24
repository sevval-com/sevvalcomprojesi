using Sevval.Application.DTOs.SlopeOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.SlopeOptions.Queries.GetSlopeOptions;

namespace Sevval.Application.Interfaces.IService
{
    public interface ISlopeOptionsService
    {
        Task<ApiResponse<List<GetSlopeOptionsQueryResponse>>> GetSlopeOptionsAsync(GetSlopeOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
