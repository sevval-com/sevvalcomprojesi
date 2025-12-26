using Sevval.Application.Features.Common;
using Sevval.Application.Features.Advertising.Queries.GetAdvertisingSettings;

namespace Sevval.Application.Interfaces.IService
{
    public interface IAdvertisingService
    {
        public Task<ApiResponse<GetAdvertisingSettingsQueryResponse>> GetAdvertisingSettingsAsync(GetAdvertisingSettingsQueryRequest request, CancellationToken cancellationToken);
    }
}
