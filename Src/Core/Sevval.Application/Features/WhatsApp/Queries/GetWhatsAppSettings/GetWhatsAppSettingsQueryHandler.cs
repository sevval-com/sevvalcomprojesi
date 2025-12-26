using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.WhatsApp.Queries.GetWhatsAppSettings
{
    public class GetWhatsAppSettingsQueryHandler : IRequestHandler<GetWhatsAppSettingsQueryRequest, ApiResponse<GetWhatsAppSettingsQueryResponse>>
    {
        private readonly IWhatsAppService _whatsAppService;

        public GetWhatsAppSettingsQueryHandler(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        public async Task<ApiResponse<GetWhatsAppSettingsQueryResponse>> Handle(GetWhatsAppSettingsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _whatsAppService.GetWhatsAppSettingsAsync(request, cancellationToken);
        }
    }
}
