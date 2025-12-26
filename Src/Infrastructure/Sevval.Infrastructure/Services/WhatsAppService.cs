using Sevval.Application.Features.Common;
using Sevval.Application.Features.WhatsApp.Queries.GetWhatsAppSettings;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        public async Task<ApiResponse<GetWhatsAppSettingsQueryResponse>> GetWhatsAppSettingsAsync(GetWhatsAppSettingsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // For now, return static settings. In a real implementation, 
                // these could be retrieved from database, configuration, or settings table
                var response = new GetWhatsAppSettingsQueryResponse
                {
                    PhoneNumber = "+905551234567", // Example Turkish phone number
                    Message = "Merhaba,%20Şevval%20Emlak%20Sitesinden%20Size%20Ulaşıyorum."
                };

                return new ApiResponse<GetWhatsAppSettingsQueryResponse>
                {
                    Data = response,
                    IsSuccessfull = true,
                    Message = "WhatsApp ayarları başarıyla getirildi."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetWhatsAppSettingsQueryResponse>
                {
                    Data = null,
                    IsSuccessfull = false,
                    Message = "WhatsApp ayarları getirilirken bir hata oluştu: " + ex.Message
                };
            }
        }
    }
}
