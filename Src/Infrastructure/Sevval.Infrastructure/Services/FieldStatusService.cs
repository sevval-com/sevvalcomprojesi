using Sevval.Application.Features.Common;
using Sevval.Application.Features.FieldStatus.Queries.GetFieldStatuses;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class FieldStatusService : IFieldStatusService
    {


        public async Task<ApiResponse<GetFieldStatusesQueryResponse>> GetFieldStatusesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var fieldStatuses = new List<FieldStatusDto>
                {
                    new FieldStatusDto
                    {
                        Value = "Satılık",
                        DisplayName = "Satılık",
                        Description = "Satılık tarla ilanları"
                    },
                    new FieldStatusDto
                    {
                        Value = "Kiralık",
                        DisplayName = "Kiralık",
                        Description = "Kiralık tarla ilanları"
                    },
                    new FieldStatusDto
                    {
                        Value = "Kat Karşılığı",
                        DisplayName = "Kat Karşılığı",
                        Description = "Kat karşılığı tarla ilanları"
                    }
                };

                var response = new GetFieldStatusesQueryResponse
                {
                    FieldStatuses = fieldStatuses
                };

                return new ApiResponse<GetFieldStatusesQueryResponse>
                {
                    IsSuccessfull = true,
                    Message = "Tarla durumları başarıyla getirildi.",
                    Data = response
                };
            }
            catch (System.Exception ex)
            {
                return new ApiResponse<GetFieldStatusesQueryResponse>
                {
                    IsSuccessfull = false,
                    Message = "Tarla durumları getirilirken bir hata oluştu: " + ex.Message
                };
            }
        }
    }
}
