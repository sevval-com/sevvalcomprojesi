using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.WhatsApp.Queries.GetWhatsAppSettings;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WhatsAppController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("settings")]
        [SwaggerOperation(Summary = "WhatsApp ayarlarını getirir", Description = "WhatsApp için telefon numarası ve mesaj ayarlarını döndürür")]
        public async Task<IActionResult> GetWhatsAppSettings([FromQuery]GetWhatsAppSettingsQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
