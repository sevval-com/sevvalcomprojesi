using Sevval.Application.DTOs.GardenStatus;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.GardenStatus.Queries.GetGardenStatuses;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services;

public class GardenStatusService : IGardenStatusService
{

    public async Task<ApiResponse<GetGardenStatusesQueryResponse>> GetGardenStatusesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var gardenStatuses = new List<GardenStatusDTO>
            {
                new GardenStatusDTO
                {
                    Value = "Satılık",
                    DisplayName = "Satılık",
                    Description = "Satılık bahçe ilanları"
                },
                new GardenStatusDTO
                {
                    Value = "Kiralık",
                    DisplayName = "Kiralık",
                    Description = "Kiralık bahçe ilanları"
                },
                new GardenStatusDTO
                {
                    Value = "Kat Karşılığı",
                    DisplayName = "Kat Karşılığı",
                    Description = "Kat karşılığı bahçe ilanları"
                }
            };

            var response = new GetGardenStatusesQueryResponse
            {
                GardenStatuses = gardenStatuses,
                Message = "Bahçe durumları başarıyla getirildi."
            };

            return new ApiResponse<GetGardenStatusesQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Bahçe durumları başarıyla getirildi.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetGardenStatusesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Bahçe durumları getirilirken bir hata oluştu.",
                Data = new GetGardenStatusesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
