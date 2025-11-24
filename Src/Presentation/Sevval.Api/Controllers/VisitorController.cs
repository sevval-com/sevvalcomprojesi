using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount;
using Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount;
using Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VisitorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(GetActiveVisitorCountQueryRequest.Route)]
        [SwaggerOperation(Summary = "Aktif ziyaretçi sayısını getirir")]
        public async Task<IActionResult> GetActiveVisitorCount([FromRoute]GetActiveVisitorCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet(GetTotalVisitorCountQueryRequest.Route)]
        [SwaggerOperation(Summary = "Toplam ziyaretçi sayısını getirir")]
        public async Task<IActionResult> GetTotalVisitorCount([FromRoute] GetTotalVisitorCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost(IncreaseVisitorCountCommandRequest.Route)]
        [SwaggerOperation(Summary = "Ziyaretçi sayısını artırır")]
        public async Task<IActionResult> IncreaseVisitorCount([FromBody] IncreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
        {
            // IP adresini otomatik olarak al
            if (string.IsNullOrEmpty(request.IpAddress))
            {
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            
            // User Agent'ı otomatik olarak al
            if (string.IsNullOrEmpty(request.UserAgent))
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            }

            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost(DecreaseVisitorCountCommandRequest.Route)]
        [SwaggerOperation(Summary = "Ziyaretçi sayısını azaltır")]
        public async Task<IActionResult> DecreaseVisitorCount([FromBody] DecreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
        {
            // IP adresini otomatik olarak al
            if (string.IsNullOrEmpty(request.IpAddress))
            {
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            
            // User Agent'ı otomatik olarak al
            if (string.IsNullOrEmpty(request.UserAgent))
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            }

            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
