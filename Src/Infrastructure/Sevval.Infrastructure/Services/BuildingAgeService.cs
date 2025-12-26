using Sevval.Application.DTOs.BuildingAge;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.BuildingAge.Queries.GetBuildingAges;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services;

public class BuildingAgeService : IBuildingAgeService
{
    public async Task<ApiResponse<GetBuildingAgesQueryResponse>> GetBuildingAgesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var buildingAges = new List<BuildingAgeDTO>
            {
                new BuildingAgeDTO { Value = "", Text = "Seçiniz" },
                new BuildingAgeDTO { Value = "0", Text = "0" },
                new BuildingAgeDTO { Value = "1", Text = "1" },
                new BuildingAgeDTO { Value = "2", Text = "2" },
                new BuildingAgeDTO { Value = "3", Text = "3" },
                new BuildingAgeDTO { Value = "4", Text = "4" },
                new BuildingAgeDTO { Value = "5-10 arası", Text = "5-10 arası" },
                new BuildingAgeDTO { Value = "11-15 arası", Text = "11-15 arası" },
                new BuildingAgeDTO { Value = "16-20 arası", Text = "16-20 arası" },
                new BuildingAgeDTO { Value = "21-25 arası", Text = "21-25 arası" },
                new BuildingAgeDTO { Value = "26-30 arası", Text = "26-30 arası" },
                new BuildingAgeDTO { Value = "30 ve üzeri", Text = "30 ve üzeri" },
                new BuildingAgeDTO { Value = "40 ve üzeri", Text = "40 ve üzeri" },
                new BuildingAgeDTO { Value = "50 ve üzeri", Text = "50 ve üzeri" }
            };

            return new ApiResponse<GetBuildingAgesQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Bina yaşı seçenekleri başarıyla getirildi.",
                Data = new GetBuildingAgesQueryResponse
                {
                    BuildingAges = buildingAges,
                    Message = "Bina yaşı seçenekleri başarıyla getirildi."
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetBuildingAgesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Bina yaşı seçenekleri getirilirken bir hata oluştu.",
                Data = new GetBuildingAgesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
