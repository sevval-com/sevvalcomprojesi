using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AboutUsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AboutUsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Kullanıcı veya şirket hakkında bilgileri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si (opsiyonel)</param>
        /// <param name="companyName">Şirket adı (opsiyonel)</param>
        /// <param name="email">Email adresi (opsiyonel)</param>
        /// <returns>Hakkımızda bilgileri</returns>
        [HttpGet(GetAboutUsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Firmanın Hakkımızda bilgilerini getirir", Description = "Firmanın Hakkımızda bilgilerini getirir. Kullanıcı veya Şirket hakkında bilgileri getirir.")]
        public async Task<IActionResult> GetAboutUs([FromQuery] GetAboutUsQueryRequest request,CancellationToken cancellationToken)
        {
           
            var response = await _mediator.Send(request,cancellationToken);
            
            if (response.IsSuccessfull)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }


         
    }
}
