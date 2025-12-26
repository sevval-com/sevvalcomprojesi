using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.FieldStatus.Queries.GetFieldStatuses;
using Sevval.Application.Features.FieldType.Queries.GetFieldTypes;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FieldController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tarla durumlarını getirir (Satılık, Kiralık, Kat Karşılığı)
        /// </summary>
        /// <returns>Tarla durumları listesi</returns>
        [HttpGet(GetFieldStatusesQueryRequest.Route)]
        [SwaggerOperation(Summary = "Tarla durumları listesi", Description = "Tarla durumlarını getirir (Satılık, Kiralık, Devren Satılık, Devren Kiralık)")]
        public async Task<IActionResult> GetFieldStatuses([FromQuery]GetFieldStatusesQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);


            return Ok(response);


        }

        /// <summary>
        /// Tarla tiplerini getirir (Ayçiçeği, Bakla, Bamya, Bağ, vb.)
        /// </summary>
        /// <returns>Tarla tipleri listesi</returns>
        [HttpGet(GetFieldTypesQueryRequest.Route)]
        [SwaggerOperation(Summary = "Tarla türleri listesi", Description = "Tarla türlerini getirir")]
        public async Task<IActionResult> GetFieldTypes([FromQuery]GetFieldTypesQueryRequest request,CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response.IsSuccessfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
