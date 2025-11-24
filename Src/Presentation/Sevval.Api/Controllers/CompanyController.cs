using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Company.Queries.GetCompanyByName;
using Sevval.Application.Features.Company.Queries.GetTotalCompanyCount;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// </summary>
        /// <param name="companyName">Aranacak şirket adı</param>
        /// <param name="cancellationToken">İptal token'ı</param>
        /// <returns>Şirket bilgileri</returns>
        [HttpGet(GetCompaniesQueryRequest.Route)]
        [SwaggerOperation(Summary = "Şirket bilgilerini getirir", Description = "Şirket bilgilerini getirir. Aranacak Şirket adı ile kullanılır.")]
        public async Task<IActionResult> GetCompanies([FromQuery] GetCompaniesQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Toplam firma sayısını getirir
        /// </summary>
        /// <param name="status">firma durumu (opsiyonel)</param>
        /// <param name="companyName">Şirket adı (opsiyonel)</param>
        /// <returns>Toplam firma sayısı</returns>
        [HttpGet(GetTotalCompanyCountQueryRequest.Route)]
        [SwaggerOperation(Summary = "Toplam firma sayısını getirir", Description = "Toplam firma sayısını getirir.")]
        public async Task<IActionResult> GetTotalCompanyCount([FromQuery] GetTotalCompanyCountQueryRequest request, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
