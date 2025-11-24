using Sevval.Application.Features.Common;
using Sevval.Application.Features.SlopeOptions.Queries.GetSlopeOptions;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class SlopeOptionsService : ISlopeOptionsService
    {
        public async Task<ApiResponse<List<GetSlopeOptionsQueryResponse>>> GetSlopeOptionsAsync(GetSlopeOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var slopeOptions = new List<GetSlopeOptionsQueryResponse>
                {
                    new GetSlopeOptionsQueryResponse { Value = "", Text = "Seçiniz" },
                    new GetSlopeOptionsQueryResponse { Value = "Düz", Text = "Düz" },
                    new GetSlopeOptionsQueryResponse { Value = "Hafif Meyilli", Text = "Hafif Meyilli" },
                    new GetSlopeOptionsQueryResponse { Value = "Meyilli", Text = "Meyilli" }
                };

                return new ApiResponse<List<GetSlopeOptionsQueryResponse>>
                {
                    IsSuccessfull = true,
                    Message = "Eğim seçenekleri básarıyla getirildi.",
                    Data = slopeOptions,
                    Code = 200
                };
            }
            catch (Exception)
            {
                throw new Exception("Eğim seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
