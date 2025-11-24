using Sevval.Application.Features.Common;
using Sevval.Application.Features.DistanceToSettlementOptions.Queries.GetDistanceToSettlementOptions;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class DistanceToSettlementOptionsService : IDistanceToSettlementOptionsService
    {
        public async Task<ApiResponse<List<GetDistanceToSettlementOptionsQueryResponse>>> GetDistanceToSettlementOptionsAsync(GetDistanceToSettlementOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var distanceToSettlementOptions = new List<GetDistanceToSettlementOptionsQueryResponse>
                {
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "", Text = "Seçiniz" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "0-100", Text = "0-100 m" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "100-300", Text = "100-300 m" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "300-500", Text = "300-500 m" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "500-1 KM", Text = "500 m - 1 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "2 KM", Text = "2 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "3 KM", Text = "3 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "4 KM", Text = "4 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "5 KM", Text = "5 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "6 KM", Text = "6 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "7 KM", Text = "7 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "8 KM", Text = "8 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "9 KM", Text = "9 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "10 KM", Text = "10 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "11 KM", Text = "11 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "12 KM", Text = "12 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "13 KM", Text = "13 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "14 KM", Text = "14 KM" },
                    new GetDistanceToSettlementOptionsQueryResponse { Value = "15 KM", Text = "15 KM" }
                };

                return new ApiResponse<List<GetDistanceToSettlementOptionsQueryResponse>>
                {
                    IsSuccessfull = true,
                    Data = distanceToSettlementOptions,
                    Message = "Yerleşim yerine uzaklık seçenekleri başarıyla getirildi.",
                    Code = 200
                };
            }
            catch (Exception)
            {
                throw new Exception("Yerleşim yerine uzaklık seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
