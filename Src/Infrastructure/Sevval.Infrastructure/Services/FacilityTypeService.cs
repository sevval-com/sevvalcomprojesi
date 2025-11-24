using Sevval.Application.Features.Common;
using Sevval.Application.Features.FacilityType.Queries.GetFacilityTypes;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class FacilityTypeService : IFacilityTypeService
    {


        public async Task<ApiResponse<List<GetFacilityTypesQueryResponse>>> GetFacilityTypesAsync(GetFacilityTypesQueryRequest request, CancellationToken cancellationToken = default)
        {

            var facilityTypes = new List<GetFacilityTypesQueryResponse>
            {
                new GetFacilityTypesQueryResponse
                {
                    Value = "1 Yıldızlı Otel",
                    DisplayName = "1 Yıldızlı Otel",
                    Description = "1 yıldızlı otel tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "2 Yıldızlı Otel",
                    DisplayName = "2 Yıldızlı Otel",
                    Description = "2 yıldızlı otel tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "3 Yıldızlı Otel",
                    DisplayName = "3 Yıldızlı Otel",
                    Description = "3 yıldızlı otel tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "4 Yıldızlı Otel",
                    DisplayName = "4 Yıldızlı Otel",
                    Description = "4 yıldızlı otel tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "5 Yıldızlı Otel",
                    DisplayName = "5 Yıldızlı Otel",
                    Description = "5 yıldızlı otel tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Apart",
                    DisplayName = "Apart",
                    Description = "Apart tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Bungalov",
                    DisplayName = "Bungalov",
                    Description = "Bungalov tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Butik Otel",
                    DisplayName = "Butik Otel",
                    Description = "Butik otel tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Dağ Evi",
                    DisplayName = "Dağ Evi",
                    Description = "Dağ evi tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Devremülk",
                    DisplayName = "Devremülk",
                    Description = "Devremülk tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Kamp Alanı",
                    DisplayName = "Kamp Alanı",
                    Description = "Kamp alanı tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Tiny House",
                    DisplayName = "Tiny House",
                    Description = "Tiny house tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Yazlık",
                    DisplayName = "Yazlık",
                    Description = "Yazlık tesisi"
                },
                new GetFacilityTypesQueryResponse
                {
                    Value = "Diğer",
                    DisplayName = "Diğer",
                    Description = "Diğer tesis türleri"
                }
            };


            return new ApiResponse<List<GetFacilityTypesQueryResponse>>
            {
                IsSuccessfull = true,
                Message = "Tesis türleri başarıyla getirildi.",
                Data = facilityTypes
            };
        }
    }
}
