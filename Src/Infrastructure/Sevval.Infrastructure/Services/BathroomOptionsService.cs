using Sevval.Application.Features.BathroomOptions.Queries.GetBathroomOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class BathroomOptionsService : IBathroomOptionsService
    {
        public async Task<ApiResponse<List<GetBathroomOptionsQueryResponse>>> GetBathroomOptionsAsync(GetBathroomOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var bathroomOptions = new List<GetBathroomOptionsQueryResponse>
                {
                    new GetBathroomOptionsQueryResponse { Value = "", Text = "Seçiniz" },
                    new GetBathroomOptionsQueryResponse { Value = "Yok", Text = "Yok" },
                    new GetBathroomOptionsQueryResponse { Value = "1", Text = "1" },
                    new GetBathroomOptionsQueryResponse { Value = "2", Text = "2" },
                    new GetBathroomOptionsQueryResponse { Value = "3", Text = "3" },
                    new GetBathroomOptionsQueryResponse { Value = "4", Text = "4" },
                    new GetBathroomOptionsQueryResponse { Value = "5", Text = "5" },
                    new GetBathroomOptionsQueryResponse { Value = "6", Text = "6" },
                    new GetBathroomOptionsQueryResponse { Value = "7 ve üzeri", Text = "7 ve üzeri" }
                };

                return new ApiResponse<List<GetBathroomOptionsQueryResponse>>
                {
                    Code = 200,
                    IsSuccessfull = true,
                    Message = "Banyo sayısı seçenekleri başarıyla getirildi.",
                    Data = bathroomOptions
                };
            }
            catch (Exception)
            {
                throw new Exception("Banyo sayısı seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
