using Sevval.Application.DTOs.PropertyType;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.PropertyType.Queries.GetPropertyTypes;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services;

public class PropertyTypeService : IPropertyTypeService
{

    public async Task<ApiResponse<GetPropertyTypesQueryResponse>> GetPropertyTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var propertyTypes = new List<PropertyTypeDTO>
            {
                new PropertyTypeDTO
                {
                    Value = "Daire",
                    DisplayName = "Daire",
                    Description = "Apartman dairesi"
                },
                new PropertyTypeDTO
                {
                    Value = "Müstakil Ev",
                    DisplayName = "Müstakil Ev",
                    Description = "Müstakil ev"
                },
                new PropertyTypeDTO
                {
                    Value = "Köy Evi",
                    DisplayName = "Köy Evi",
                    Description = "Köy evi"
                },
                new PropertyTypeDTO
                {
                    Value = "Eski, Kargir Ev",
                    DisplayName = "Eski, Kargir Ev",
                    Description = "Eski, kargir ev"
                },
                new PropertyTypeDTO
                {
                    Value = "Dağ Evi",
                    DisplayName = "Dağ Evi",
                    Description = "Dağ evi"
                },
                new PropertyTypeDTO
                {
                    Value = "Kerpiç Ev",
                    DisplayName = "Kerpiç Ev",
                    Description = "Kerpiç ev"
                },
                new PropertyTypeDTO
                {
                    Value = "Samanlık",
                    DisplayName = "Samanlık",
                    Description = "Samanlık"
                },
                new PropertyTypeDTO
                {
                    Value = "Kulübe",
                    DisplayName = "Kulübe",
                    Description = "Kulübe"
                },
                new PropertyTypeDTO
                {
                    Value = "Konteyner",
                    DisplayName = "Konteyner",
                    Description = "Konteyner ev"
                },
                new PropertyTypeDTO
                {
                    Value = "Prefabrik",
                    DisplayName = "Prefabrik",
                    Description = "Prefabrik ev"
                },
                new PropertyTypeDTO
                {
                    Value = "Bungalow",
                    DisplayName = "Bungalow",
                    Description = "Bungalow"
                },
                new PropertyTypeDTO
                {
                    Value = "Tiny House",
                    DisplayName = "Tiny House",
                    Description = "Tiny house"
                },
                new PropertyTypeDTO
                {
                    Value = "Loft Daire",
                    DisplayName = "Loft Daire",
                    Description = "Loft daire"
                },
                new PropertyTypeDTO
                {
                    Value = "Bina",
                    DisplayName = "Bina",
                    Description = "Bina"
                },
                new PropertyTypeDTO
                {
                    Value = "Devremülk",
                    DisplayName = "Devremülk",
                    Description = "Devremülk"
                },
                new PropertyTypeDTO
                {
                    Value = "Çiftlik Evi",
                    DisplayName = "Çiftlik Evi",
                    Description = "Çiftlik evi"
                },
                new PropertyTypeDTO
                {
                    Value = "Kooperatif",
                    DisplayName = "Kooperatif",
                    Description = "Kooperatif"
                },
                new PropertyTypeDTO
                {
                    Value = "Villa",
                    DisplayName = "Villa",
                    Description = "Villa"
                },
                new PropertyTypeDTO
                {
                    Value = "Köşk",
                    DisplayName = "Köşk",
                    Description = "Köşk"
                },
                new PropertyTypeDTO
                {
                    Value = "Konak",
                    DisplayName = "Konak",
                    Description = "Konak"
                },
                new PropertyTypeDTO
                {
                    Value = "Yalı",
                    DisplayName = "Yalı",
                    Description = "Yalı"
                },
                new PropertyTypeDTO
                {
                    Value = "Ev ve Ahır",
                    DisplayName = "Ev ve Ahır",
                    Description = "Ev ve ahır"
                },
                new PropertyTypeDTO
                {
                    Value = "Rezidans",
                    DisplayName = "Rezidans",
                    Description = "Rezidans"
                },
                new PropertyTypeDTO
                {
                    Value = "Diğer",
                    DisplayName = "Diğer",
                    Description = "Diğer mülk tipleri"
                }
            };

            var response = new GetPropertyTypesQueryResponse
            {
                PropertyTypes = propertyTypes,
                Message = "Mülk tipleri başarıyla getirildi."
            };

            return new ApiResponse<GetPropertyTypesQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Mülk tipleri başarıyla getirildi.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetPropertyTypesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Mülk tipleri getirilirken bir hata oluştu.",
                Data = new GetPropertyTypesQueryResponse
                {
                    Message = ex.Message
                }
            };
        }
    }
}
