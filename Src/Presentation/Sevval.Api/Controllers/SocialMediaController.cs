using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.SocialMedia.Queries.GetSocialMedia;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SocialMediaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Sosyal medya hesap bilgilerini getirir
        /// </summary>
        /// <returns>Sosyal medya hesap bilgileri</returns>
        [HttpGet(GetSocialMediaQueryRequest.Route)]
        [SwaggerOperation(Summary = "Sosyal medya hesap bilgilerini getirir", Description = "Bu endpoint, sosyal medya hesap bilgilerini döner.")]
        public async Task<IActionResult> GetSocialMedia([FromRoute]GetSocialMediaQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
