using Sevval.Application.DTOs.HeatingSystemOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.HeatingSystemOptions.Queries.GetHeatingSystemOptions;

namespace Sevval.Application.Interfaces.IService
{
    public interface IHeatingSystemOptionsService
    {
        Task<ApiResponse<List<GetHeatingSystemOptionsQueryResponse>>> GetHeatingSystemOptionsAsync(GetHeatingSystemOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
