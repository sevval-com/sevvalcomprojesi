using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Advertising.Queries.GetAdvertisingSettings
{
    public class GetAdvertisingSettingsQueryHandler : IRequestHandler<GetAdvertisingSettingsQueryRequest, ApiResponse<GetAdvertisingSettingsQueryResponse>>
    {
        private readonly IAdvertisingService _advertisingService;

        public GetAdvertisingSettingsQueryHandler(IAdvertisingService advertisingService)
        {
            _advertisingService = advertisingService;
        }

        public async Task<ApiResponse<GetAdvertisingSettingsQueryResponse>> Handle(GetAdvertisingSettingsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _advertisingService.GetAdvertisingSettingsAsync(request, cancellationToken);
        }
    }
}
