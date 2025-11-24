using Sevval.Application.Features.BusinessStatus.Queries.GetBusinessStatuses;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class BusinessStatusService : IBusinessStatusService
    {

        public async Task<ApiResponse<List<GetBusinessStatusesQueryResponse>>> GetBusinessStatusesAsync(GetBusinessStatusesQueryRequest request, CancellationToken cancellationToken = default)
        {
            
                var businessStatuses = new List<GetBusinessStatusesQueryResponse>
                {
                    new GetBusinessStatusesQueryResponse
                    {
                        Value = "Satılık",
                        DisplayName = "Satılık",
                        Description = "Satılık iş yeri ilanları"
                    },
                    new GetBusinessStatusesQueryResponse
                    {
                        Value = "Kiralık",
                        DisplayName = "Kiralık",
                        Description = "Kiralık iş yeri ilanları"
                    },
                    new GetBusinessStatusesQueryResponse
                    {
                        Value = "Devren Satılık",
                        DisplayName = "Devren Satılık",
                        Description = "Devren satılık iş yeri ilanları"
                    },
                    new GetBusinessStatusesQueryResponse
                    {
                        Value = "Devren Kiralık",
                        DisplayName = "Devren Kiralık",
                        Description = "Devren kiralık iş yeri ilanları"
                    }
                };

                return new ApiResponse<List<GetBusinessStatusesQueryResponse>>
                {
                    IsSuccessfull = true,
                    Message = "İş yeri durumları başarıyla getirildi.",
                    Data = businessStatuses
                };

           
        }
    }
}
