using Sevval.Application.Features.Common;
using Sevval.Application.Features.FacilityStatus.Queries.GetFacilityStatuses;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class FacilityStatusService : IFacilityStatusService
    {

        public async Task<ApiResponse<List<GetFacilityStatusesQueryResponse>>> GetFacilityStatusesAsync(GetFacilityStatusesQueryRequest request, CancellationToken cancellationToken = default)
        {

            var facilityStatuses = new List<GetFacilityStatusesQueryResponse>
                {
                    new GetFacilityStatusesQueryResponse
                    {
                        Value = "Satılık",
                        DisplayName = "Satılık",
                        Description = "Satılık tesis ilanları"
                    },
                    new GetFacilityStatusesQueryResponse
                    {
                        Value = "Kiralık",
                        DisplayName = "Kiralık",
                        Description = "Kiralık tesis ilanları"
                    },
                    new GetFacilityStatusesQueryResponse
                    {
                        Value = "Devren Satılık",
                        DisplayName = "Devren Satılık",
                        Description = "Devren satılık tesis ilanları"
                    },
                    new GetFacilityStatusesQueryResponse
                    {
                        Value = "Devren Kiralık",
                        DisplayName = "Devren Kiralık",
                        Description = "Devren kiralık tesis ilanları"
                    }
                };

            return new ApiResponse<List<GetFacilityStatusesQueryResponse>>
            {
                IsSuccessfull = true,
                Message = "Tesis durumları başarıyla getirildi.",
                Data = facilityStatuses
            };
        }
    }
}
