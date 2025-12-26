using Sevval.Application.Features.Common;
using Sevval.Application.Features.WhatsApp.Queries.GetWhatsAppSettings;

namespace Sevval.Application.Interfaces.IService
{
    public interface IWhatsAppService
    {
        public Task<ApiResponse<GetWhatsAppSettingsQueryResponse>> GetWhatsAppSettingsAsync(GetWhatsAppSettingsQueryRequest request, CancellationToken cancellationToken);
    }
}
