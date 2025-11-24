using Sevval.Application.DTOs.LandType;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.LandType.Queries.GetLandTypes;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services;

public class LandTypeService :  ILandTypeService
{


    public async Task<ApiResponse<GetLandTypesQueryResponse>> GetLandTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var landTypes = new List<LandTypeDTO>
            {
                new LandTypeDTO
                {
                    Value = "A-Lejantlı Arsa",
                    DisplayName = "A-Lejantlı Arsa",
                    Description = "A-Lejantlı arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Bahçe",
                    DisplayName = "Bahçe",
                    Description = "Bahçe arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Çiftlik",
                    DisplayName = "Çiftlik",
                    Description = "Çiftlik arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Depo Ve Antrepo",
                    DisplayName = "Depo Ve Antrepo",
                    Description = "Depo ve antrepo arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Eğitim",
                    DisplayName = "Eğitim",
                    Description = "Eğitim amaçlı arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Enerji Depolama",
                    DisplayName = "Enerji Depolama",
                    Description = "Enerji depolama arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Hastane",
                    DisplayName = "Hastane",
                    Description = "Hastane arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Kamu Alanı",
                    DisplayName = "Kamu Alanı",
                    Description = "Kamu alanı arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Karma",
                    DisplayName = "Karma",
                    Description = "Karma arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Konut",
                    DisplayName = "Konut",
                    Description = "Konut arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Konut + Ticaret",
                    DisplayName = "Konut + Ticaret",
                    Description = "Konut + ticaret arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Muhtelif Arsa",
                    DisplayName = "Muhtelif Arsa",
                    Description = "Muhtelif arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Özel Kullanım",
                    DisplayName = "Özel Kullanım",
                    Description = "Özel kullanım arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Petrol Lejantı",
                    DisplayName = "Petrol Lejantı",
                    Description = "Petrol lejantı arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Sanayi",
                    DisplayName = "Sanayi",
                    Description = "Sanayi arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Sağlık",
                    DisplayName = "Sağlık",
                    Description = "Sağlık arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Sera",
                    DisplayName = "Sera",
                    Description = "Sera arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Sit Alanı",
                    DisplayName = "Sit Alanı",
                    Description = "Sit alanı arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Spor Alanı",
                    DisplayName = "Spor Alanı",
                    Description = "Spor alanı arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Tarım",
                    DisplayName = "Tarım",
                    Description = "Tarım arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Ticaret",
                    DisplayName = "Ticaret",
                    Description = "Ticaret arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Toplu Konut",
                    DisplayName = "Toplu Konut",
                    Description = "Toplu konut arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Turizm",
                    DisplayName = "Turizm",
                    Description = "Turizm arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Turizm Arsa",
                    DisplayName = "Turizm Arsa",
                    Description = "Turizm arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Yeşil Alan",
                    DisplayName = "Yeşil Alan",
                    Description = "Yeşil alan arsa ilanları"
                },
                new LandTypeDTO
                {
                    Value = "Diğer",
                    DisplayName = "Diğer",
                    Description = "Diğer arsa ilanları"
                }
            };

            var response = new GetLandTypesQueryResponse
            {
                LandTypes = landTypes,
                Message = "Arsa tipleri başarıyla getirildi."
            };

            return new ApiResponse<GetLandTypesQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Arsa tipleri başarıyla getirildi.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetLandTypesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Arsa tipleri getirilirken bir hata oluştu.",
                Data = new GetLandTypesQueryResponse
                {
                    Message = ex.Message,
                    LandTypes = new List<LandTypeDTO>()
                }
            };
        }
    }
}
