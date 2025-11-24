using Sevval.Application.DTOs.FloorOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.FloorOptions.Queries.GetFloorOptions;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class FloorOptionsService : IFloorOptionsService
    {
        public async Task<ApiResponse<List<GetFloorOptionsQueryResponse>>> GetFloorOptionsAsync(GetFloorOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var floorOptions = new List<GetFloorOptionsQueryResponse>
                {
                    new GetFloorOptionsQueryResponse { Value = "", Text = "Seçiniz" },
                    new GetFloorOptionsQueryResponse { Value = "Giriş Altı Kot 4", Text = "Giriş Altı Kot 4" },
                    new GetFloorOptionsQueryResponse { Value = "Giriş Altı Kot 3", Text = "Giriş Altı Kot 3" },
                    new GetFloorOptionsQueryResponse { Value = "Giriş Altı Kot 2", Text = "Giriş Altı Kot 2" },
                    new GetFloorOptionsQueryResponse { Value = "Giriş Altı Kot 1", Text = "Giriş Altı Kot 1" },
                    new GetFloorOptionsQueryResponse { Value = "Bodrum Kat", Text = "Bodrum Kat" },
                    new GetFloorOptionsQueryResponse { Value = "Zemin Kat", Text = "Zemin Kat" },
                    new GetFloorOptionsQueryResponse { Value = "Bahçe Katı", Text = "Bahçe Katı" },
                    new GetFloorOptionsQueryResponse { Value = "Giriş Katı", Text = "Giriş Katı" },
                    new GetFloorOptionsQueryResponse { Value = "Yüksek Giriş", Text = "Yüksek Giriş" },
                    new GetFloorOptionsQueryResponse { Value = "Ara Kat", Text = "Ara Kat" },
                    new GetFloorOptionsQueryResponse { Value = "Müstakil", Text = "Müstakil" },
                    new GetFloorOptionsQueryResponse { Value = "Villa Tipi", Text = "Villa Tipi" },
                    new GetFloorOptionsQueryResponse { Value = "Çatı Katı", Text = "Çatı Katı" },
                    new GetFloorOptionsQueryResponse { Value = "En Üst Kat", Text = "En Üst Kat" }
                };

                // Add numbered floors from 1 to 49
                for (int i = 1; i <= 49; i++)
                {
                    floorOptions.Add(new GetFloorOptionsQueryResponse { Value = i.ToString(), Text = i.ToString() });
                }

                // Add the final option
                floorOptions.Add(new GetFloorOptionsQueryResponse { Value = "50 ve üzeri", Text = "50 ve üzeri" });

                return new ApiResponse<List<GetFloorOptionsQueryResponse>>
                {
                    Code = 200,
                    IsSuccessfull = true,
                    Message = "Kat seçenekleri başarıyla getirildi.",
                    Data = floorOptions
                };
            }
            catch (Exception)
            {
                throw new Exception("Kat seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
