using Sevval.Application.Features.Common;
using Sevval.Application.Features.Advertising.Queries.GetAdvertisingSettings;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class AdvertisingService : IAdvertisingService
    {
        public async Task<ApiResponse<GetAdvertisingSettingsQueryResponse>> GetAdvertisingSettingsAsync(GetAdvertisingSettingsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // For now, return static settings based on the provided example
                // In a real implementation, these could be retrieved from database, configuration, or settings table
                var response = new GetAdvertisingSettingsQueryResponse
                {
                    ImagePath = "~/sablon/img/lavanta.gif",
                    DestinationUrl = "https://www.instagram.com/tunalavanta/"
                };

                return new ApiResponse<GetAdvertisingSettingsQueryResponse>
                {
                    Data = response,
                    IsSuccessfull = true,
                    Message = "Reklam ayarları başarıyla getirildi."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetAdvertisingSettingsQueryResponse>
                {
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Reklam ayarları getirilirken bir hata oluştu: " + ex.Message
                };
            }
        }
    }
}
