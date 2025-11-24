using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Advertising.Queries.GetAdvertisingSettings
{
    public class GetAdvertisingSettingsQueryRequest : IRequest<ApiResponse<GetAdvertisingSettingsQueryResponse>>
    {
        public const string Route = "/api/v1/advertising/settings";
    }
}
