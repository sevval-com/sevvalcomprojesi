using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.BusinessStatus.Queries.GetBusinessStatuses;
using Sevval.Application.Features.BusinessType.Queries.GetBusinessTypes;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BusinessController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// İş yeri durumlarını getirir (Satılık, Kiralık, Devren Satılık, Devren Kiralık)
        /// </summary>
        /// <returns>İş yeri durumları listesi</returns>
        [HttpGet(GetBusinessStatusesQueryRequest.Route)]
        [SwaggerOperation(Summary = "İş yeri durumları listesi", Description = "İş yeri durumlarını getirir (Satılık, Kiralık, Devren Satılık, Devren Kiralık)")]
        public async Task<IActionResult> GetBusinessStatuses([FromQuery]GetBusinessStatusesQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet(GetBusinessTypesQueryRequest.Route)]
        [SwaggerOperation(Summary = "İş yeri türleri listesi", Description = "İş yeri türlerini getirir")]

        public async Task<IActionResult> GetBusinessTypes([FromQuery] GetBusinessTypesQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);

        }
    }
}
