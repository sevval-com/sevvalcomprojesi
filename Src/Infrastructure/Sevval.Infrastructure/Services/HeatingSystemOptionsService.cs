using Sevval.Application.DTOs.HeatingSystemOptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.HeatingSystemOptions.Queries.GetHeatingSystemOptions;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class HeatingSystemOptionsService : IHeatingSystemOptionsService
    {
        public async Task<ApiResponse<List<GetHeatingSystemOptionsQueryResponse>>> GetHeatingSystemOptionsAsync(GetHeatingSystemOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var heatingSystemOptions = new List<GetHeatingSystemOptionsQueryResponse>
                {
                    new GetHeatingSystemOptionsQueryResponse { Value = "", Text = "Seçiniz" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Yok", Text = "Yok" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Soba", Text = "Soba" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Doğalgaz Sobası", Text = "Doğalgaz Sobası" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Kat Kaloriferi", Text = "Kat Kaloriferi" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Merkezi", Text = "Merkezi" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Merkezi (Pay Ölçer)", Text = "Merkezi (Pay Ölçer)" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Kombi (Doğalgaz)", Text = "Kombi (Doğalgaz)" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Kombi (Elektrik)", Text = "Kombi (Elektrik)" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Yerden Isıtma", Text = "Yerden Isıtma" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Klima", Text = "Klima" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Fancoil Ünitesi", Text = "Fancoil Ünitesi" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Güneş Enerjisi", Text = "Güneş Enerjisi" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Elektrikli Radyatör", Text = "Elektrikli Radyatör" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Jeotermal", Text = "Jeotermal" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Şömine", Text = "Şömine" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "VRV", Text = "VRV" },
                    new GetHeatingSystemOptionsQueryResponse { Value = "Isı Pompası", Text = "Isı Pompası" }
                };

                return new ApiResponse<List<GetHeatingSystemOptionsQueryResponse>>
                {
                    Code = 200,
                    IsSuccessfull = true,
                    Message = "Isıtma sistemi seçenekleri başarıyla getirildi.",
                    Data = heatingSystemOptions
                };
            }
            catch (Exception)
            {
                throw new Exception("Isıtma sistemi seçenekleri getirilirken bir hata oluştu.");
            }
        }
    }
}
