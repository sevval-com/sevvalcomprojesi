using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Advertising.Queries.GetAdvertisingSettings;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdvertisingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdvertisingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("settings")]
        [SwaggerOperation(Summary = "Reklam ayarlarını getirir", Description = "Reklam için resim yolu ve hedef URL ayarlarını döndürür")]
        public async Task<IActionResult> GetAdvertisingSettings([FromQuery]GetAdvertisingSettingsQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
