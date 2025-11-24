using Sevval.Application.Features.Common;
using Sevval.Application.Features.RoadConditionOptions.Queries.GetRoadConditionOptions;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class RoadConditionOptionsService : IRoadConditionOptionsService
    {
        public async Task<ApiResponse<List<GetRoadConditionOptionsQueryResponse>>> GetRoadConditionOptionsAsync(GetRoadConditionOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var roadConditionOptions = new List<GetRoadConditionOptionsQueryResponse>
                {
                    new GetRoadConditionOptionsQueryResponse { Value = "", Text = "Seçiniz" },
                    new GetRoadConditionOptionsQueryResponse { Value = "Yok", Text = "Yok" },
                    new GetRoadConditionOptionsQueryResponse { Value = "Toprak Yollu", Text = "Toprak Yollu" },
                    new GetRoadConditionOptionsQueryResponse { Value = "Kadastral Yollu", Text = "Kadastral Yollu" },
                    new GetRoadConditionOptionsQueryResponse { Value = "Yola 1 Parsel", Text = "Yola 1 Parsel" }
                };

                return new ApiResponse<List<GetRoadConditionOptionsQueryResponse>>
                {
                    IsSuccessfull = true,
                    Message = "Yol durumu seçenekleri başarıyla getirildi.",
                    Data = roadConditionOptions,
                    Code = 200
                };
            }
            catch (Exception)
            {
                throw new Exception("Yol durumu seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
