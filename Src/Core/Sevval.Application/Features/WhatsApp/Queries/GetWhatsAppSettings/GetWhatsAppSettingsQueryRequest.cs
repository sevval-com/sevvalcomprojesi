using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.WhatsApp.Queries.GetWhatsAppSettings
{
    public class GetWhatsAppSettingsQueryRequest : IRequest<ApiResponse<GetWhatsAppSettingsQueryResponse>>
    {
        public const string Route = "/api/v1/whatsapp/settings";
    }
}
