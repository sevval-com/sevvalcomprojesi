using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.About.Queries.GetAboutContent;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AboutController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AboutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hakkımızda sayfasının içeriğini getirir
        /// </summary>
        /// <returns>Hakkımızda içeriği</returns>
        [HttpGet(GetAboutContentQueryRequest.Route)]
        [SwaggerOperation(Summary = "Hakkımızda sayfasının içeriğini getirir", Description = "Hakkımızda sayfasının içeriğini getirir")]
        public async Task<IActionResult> GetAboutContent([FromQuery] GetAboutContentQueryRequest request)
        {
            var result = await _mediator.Send(request);

            if (result.IsSuccessfull)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
