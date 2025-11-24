using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.ContactInfo.Queries.GetContactInfo;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactInfoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContactInfoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// İletişim ve sosyal medya bilgilerini getirir
        /// </summary>
        /// <returns>İletişim bilgileri</returns>
        [HttpGet(GetContactInfoQueryRequest.Route)]
        [SwaggerOperation(Summary = "İletişim  bilgilerini getirir", Description = "Bu endpoint, iletişim  bilgilerini döner.")]
        public async Task<IActionResult> GetContactInfo([FromRoute] GetContactInfoQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
