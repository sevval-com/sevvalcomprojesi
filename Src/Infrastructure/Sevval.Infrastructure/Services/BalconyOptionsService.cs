using Sevval.Application.Features.BalconyOptions.Queries.GetBalconyOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class BalconyOptionsService : IBalconyOptionsService
    {
        public async Task<ApiResponse<List<GetBalconyOptionsQueryResponse>>> GetBalconyOptionsAsync(GetBalconyOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var balconyOptions = new List<GetBalconyOptionsQueryResponse>
                {
                    new GetBalconyOptionsQueryResponse { Value = "", Text = "Seçiniz" },
                    new GetBalconyOptionsQueryResponse { Value = "Yok", Text = "Yok" },
                    new GetBalconyOptionsQueryResponse { Value = "1", Text = "1" },
                    new GetBalconyOptionsQueryResponse { Value = "2", Text = "2" },
                    new GetBalconyOptionsQueryResponse { Value = "3", Text = "3" },
                    new GetBalconyOptionsQueryResponse { Value = "4", Text = "4" },
                    new GetBalconyOptionsQueryResponse { Value = "5", Text = "5" },
                    new GetBalconyOptionsQueryResponse { Value = "6", Text = "6" },
                    new GetBalconyOptionsQueryResponse { Value = "7 ve üzeri", Text = "7 ve üzeri" }
                };

                return new ApiResponse<List<GetBalconyOptionsQueryResponse>>
                {
                    IsSuccessfull = true,
                    Message = "Balkon seçenekleri başarıyla getirildi.",
                    Data = balconyOptions,
                    Code = 200

                };
            }
            catch (Exception)
            {
                throw new Exception("Balkon seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
