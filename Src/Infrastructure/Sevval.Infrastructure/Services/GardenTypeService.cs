using Sevval.Application.Features.Common;
using Sevval.Application.Features.GardenType.Queries.GetGardenTypes;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services;

public class GardenTypeService : IGardenTypeService
{


    public async Task<ApiResponse<List<GetGardenTypesQueryResponse>>> GetGardenTypesAsync(CancellationToken cancellationToken = default)
    {
        
            var gardenTypes = new List<GetGardenTypesQueryResponse>
            {
                new GetGardenTypesQueryResponse
                {
                    Value = "Ahududu",
                    DisplayName = "Ahududu",
                    Description = "Ahududu bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Armut",
                    DisplayName = "Armut",
                    Description = "Armut bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Avokado",
                    DisplayName = "Avokado",
                    Description = "Avokado bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Ayva",
                    DisplayName = "Ayva",
                    Description = "Ayva bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Badem",
                    DisplayName = "Badem",
                    Description = "Badem bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Böğürtlen",
                    DisplayName = "Böğürtlen",
                    Description = "Böğürtlen bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Ceviz",
                    DisplayName = "Ceviz",
                    Description = "Ceviz bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Çilek",
                    DisplayName = "Çilek",
                    Description = "Çilek bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Dut",
                    DisplayName = "Dut",
                    Description = "Dut bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Elma",
                    DisplayName = "Elma",
                    Description = "Elma bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Erik",
                    DisplayName = "Erik",
                    Description = "Erik bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Fındık",
                    DisplayName = "Fındık",
                    Description = "Fındık bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Greyfurt",
                    DisplayName = "Greyfurt",
                    Description = "Greyfurt bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Hurma",
                    DisplayName = "Hurma",
                    Description = "Hurma bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "İğde",
                    DisplayName = "İğde",
                    Description = "İğde bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Kayısı",
                    DisplayName = "Kayısı",
                    Description = "Kayısı bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Kestane",
                    DisplayName = "Kestane",
                    Description = "Kestane bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Kivi",
                    DisplayName = "Kivi",
                    Description = "Kivi bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Kızılcık",
                    DisplayName = "Kızılcık",
                    Description = "Kızılcık bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Kiraz",
                    DisplayName = "Kiraz",
                    Description = "Kiraz bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Karışık",
                    DisplayName = "Karışık",
                    Description = "Karışık meyve bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Lavanta",
                    DisplayName = "Lavanta",
                    Description = "Lavanta bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Limon",
                    DisplayName = "Limon",
                    Description = "Limon bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Mandalina",
                    DisplayName = "Mandalina",
                    Description = "Mandalina bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Muşmula",
                    DisplayName = "Muşmula",
                    Description = "Muşmula bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Muz",
                    DisplayName = "Muz",
                    Description = "Muz bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Nar",
                    DisplayName = "Nar",
                    Description = "Nar bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Portakal",
                    DisplayName = "Portakal",
                    Description = "Portakal bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Şeftali",
                    DisplayName = "Şeftali",
                    Description = "Şeftali bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Üzüm",
                    DisplayName = "Üzüm",
                    Description = "Üzüm bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Vişne",
                    DisplayName = "Vişne",
                    Description = "Vişne bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Zeytin",
                    DisplayName = "Zeytin",
                    Description = "Zeytin bahçesi"
                },
                new GetGardenTypesQueryResponse
                {
                    Value = "Diğer",
                    DisplayName = "Diğer",
                    Description = "Diğer bahçe türleri"
                }
            };

            return new ApiResponse<List<GetGardenTypesQueryResponse>>
            {
                Code = 200,
                Data = gardenTypes,
                Message = "Bahçe türleri basarıyla getirildi.",
                IsSuccessfull = true,

            };
       
    }
}
