using Sevval.Application.Features.Common;
using Sevval.Application.Features.PropertyStatus.Queries.GetPropertyStatuses;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Infrastructure.Services;

public class PropertyStatusService : IPropertyStatusService
{


    public async Task<ApiResponse<GetPropertyStatusesQueryResponse>> GetPropertyStatusesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var propertyStatuses = new List<PropertyStatusDto>
            {
                new PropertyStatusDto
                {
                    Value = "Satılık",
                    DisplayName = "Satılık",
                    Description = "Satılık konut ilanları"
                },
                new PropertyStatusDto
                {
                    Value = "Kiralık",
                    DisplayName = "Kiralık",
                    Description = "Kiralık konut ilanları"
                },
                new PropertyStatusDto
                {
                    Value = "Devren Satılık",
                    DisplayName = "Devren Satılık",
                    Description = "Devren satılık konut ilanları"
                },
                new PropertyStatusDto
                {
                    Value = "Devren Kiralık",
                    DisplayName = "Devren Kiralık",
                    Description = "Devren kiralık konut ilanları"
                }
            };

            var response = new GetPropertyStatusesQueryResponse
            {
                PropertyStatuses = propertyStatuses,
                Message = "Konut durumları başarıyla getirildi."
            };

            return new ApiResponse<GetPropertyStatusesQueryResponse>
            {
                Data = response,
                IsSuccessfull = true,
                Message = "Konut durumları başarıyla getirildi."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetPropertyStatusesQueryResponse>
            {
                IsSuccessfull = false,
                Message = "Konut durumları getirilirken bir hata oluştu: " + ex.Message
            };
        }
    }
}
