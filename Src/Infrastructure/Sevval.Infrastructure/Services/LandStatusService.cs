using Sevval.Application.DTOs.LandStatus;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.LandStatus.Queries.GetLandStatuses;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services;

public class LandStatusService : ILandStatusService
{

    public async Task<ApiResponse<GetLandStatusesQueryResponse>> GetLandStatusesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var landStatuses = new List<LandStatusDTO>
            {
                new LandStatusDTO
                {
                    Value = "Satılık",
                    DisplayName = "Satılık",
                    Description = "Satılık arsa ilanları"
                },
                new LandStatusDTO
                {
                    Value = "Kiralık",
                    DisplayName = "Kiralık",
                    Description = "Kiralık arsa ilanları"
                },
                new LandStatusDTO
                {
                    Value = "Kat Karşılığı",
                    DisplayName = "Kat Karşılığı",
                    Description = "Kat karşılığı arsa ilanları"
                }
            };

            var response = new GetLandStatusesQueryResponse
            {
                LandStatuses = landStatuses,
                Message = "Arsa durumları başarıyla getirildi."
            };

            return new ApiResponse<GetLandStatusesQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Arsa durumları başarıyla getirildi.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetLandStatusesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Arsa durumları getirilirken bir hata oluştu.",
                Data = new GetLandStatusesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
