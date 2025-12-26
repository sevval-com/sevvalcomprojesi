using Sevval.Application.DTOs.ZoningStatusOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.ZoningStatusOptions.Queries.GetZoningStatusOptions;

namespace Sevval.Application.Interfaces.IService
{
    public interface IZoningStatusOptionsService
    {
        Task<ApiResponse<List<GetZoningStatusOptionsQueryResponse>>> GetZoningStatusOptionsAsync(GetZoningStatusOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
