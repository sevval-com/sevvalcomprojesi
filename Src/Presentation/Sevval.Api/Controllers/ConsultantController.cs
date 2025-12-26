using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConsultantController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConsultantController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Şirkete ait danışmanları getirir
        /// </summary>
        /// <param name="request">Şirket danışmanları sorgu parametreleri</param>
        /// <param name="cancellationToken">İptal token'ı</param>
        /// <returns>Şirkete ait danışman listesi</returns>
        [HttpGet(GetConsultantsByCompanyQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "Şirkete ait danışmanları getirir",
            Description = "Belirtilen şirkete ait danışmanları sayfalama, sıralama ve filtreleme seçenekleri ile getirir.")]
        public async Task<IActionResult> GetConsultantsByCompany([FromQuery] GetConsultantsByCompanyQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Toplam danışman sayısını getirir
        /// </summary>
        /// <param name="request">Danışman sayısı sorgu parametreleri</param>
        /// <param name="cancellationToken">İptal token'ı</param>
        /// <returns>Toplam danışman sayısı</returns>
        [HttpGet(GetTotalConsultantCountQueryRequest.Route)]
        [SwaggerOperation(
            Summary = "Toplam danışman sayısını getirir",
            Description = "Toplam danışman sayısını opsiyonel filtreleme seçenekleri ile getirir.")]
        public async Task<IActionResult> GetTotalConsultantCount([FromQuery] GetTotalConsultantCountQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
