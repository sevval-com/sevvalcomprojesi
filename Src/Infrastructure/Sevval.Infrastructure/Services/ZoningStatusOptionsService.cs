using Sevval.Application.Features.Common;
using Sevval.Application.Features.ZoningStatusOptions.Queries.GetZoningStatusOptions;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class ZoningStatusOptionsService : IZoningStatusOptionsService
    {
        public async Task<ApiResponse<List<GetZoningStatusOptionsQueryResponse>>> GetZoningStatusOptionsAsync(GetZoningStatusOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var zoningStatusOptions = new List<GetZoningStatusOptionsQueryResponse>
                {
                    new GetZoningStatusOptionsQueryResponse { Value = "Imari Yok", Text = "İmarı Yok" },
                    new GetZoningStatusOptionsQueryResponse { Value = "Konut İmarlı", Text = "Konut İmarlı" },
                    new GetZoningStatusOptionsQueryResponse { Value = "Sanayi İmarlı", Text = "Sanayi İmarlı" },
                    new GetZoningStatusOptionsQueryResponse { Value = "Ticaret İmarlı", Text = "Ticaret İmarlı" },
                    new GetZoningStatusOptionsQueryResponse { Value = "Karma Kullanım İmarlı", Text = "Karma Kullanım İmarlı" },
                    new GetZoningStatusOptionsQueryResponse { Value = "Tarım İmarlı", Text = "Tarım İmarlı" },
                    new GetZoningStatusOptionsQueryResponse { Value = "Turizm İmarlı", Text = "Turizm İmarlı" }
                };

                return new ApiResponse<List<GetZoningStatusOptionsQueryResponse>>
                {
                    IsSuccessfull = true,
                    Message = "İmar durumu seçenekleri başarıyla getirildi.",
                    Data = zoningStatusOptions,
                    Code = 200
                };
            }
            catch (Exception)
            {
                throw new Exception("İmar durumu seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
